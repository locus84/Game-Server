using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ardalis.Specification;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.PlayerAggregate.Events;
using GameServer.Core.PlayerAggregate.Specifications;
using GameServer.Core.StatisticsAggregate;
using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;
using GameServer.Web.Extensions;
using GameServer.Web.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace GameServer.Web.Api;

public class AuthorizeController : BaseApiController
{
  private readonly UserManager<Player> _userManager;
  private readonly SignInManager<Player> _signInManager;
  private readonly IRepository<Player> _playerRepository;
  private readonly IRepository<Statistics> _statsRepository;
  private readonly IConfiguration _configuration;
  private readonly IMediator _mediator;

  public AuthorizeController(UserManager<Player> userManager, IConfiguration configuration,
    SignInManager<Player> signInManager,
    IRepository<Player> playerRepo, IRepository<Statistics> statsRepo, IMediator mediator)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _playerRepository = playerRepo;
    _statsRepository = statsRepo;
    _mediator = mediator;
    _configuration = configuration;
  }

  [HttpPost]
  public async Task<IActionResult> Login(LoginParameters parameters)
  {
    Player? user = null;
    if (!string.IsNullOrEmpty(parameters.UserName))
    {
      user = await _userManager.FindByNameAsync(parameters.UserName!);
    }
    else
    {
      if (string.IsNullOrEmpty(parameters.Mobile))
      {
        return BadRequest(("You need to provide one of the Username Or Mobile fields for logging in."));
      }

      user = await _playerRepository.FirstOrDefaultAsync(new PlayerByMobileSpec(parameters.Mobile));
    }
    
    if (user == null) return BadRequest("User does not exist");
    
    if (parameters.TokenBasedAuth)
    {
      return Ok(new LoginResult { UserId = user.Id, UserName = user.UserName, Token = CreateJWT(user) });
    }

    var singInResult = await _signInManager.CheckPasswordSignInAsync(user, parameters.Password!, false);
    if (!singInResult.Succeeded) return BadRequest("Invalid password");

    var authProperties = new AuthenticationProperties
    {
      IsPersistent = true,//parameters.RememberMe,
      ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_configuration.GetValue<int>("CookieMinutes")),
      RedirectUri = this.Request.Host.Value
    };
    var claims = new List<Claim>
     {
       new Claim(ClaimTypes.Name, user.UserName!),
       new Claim(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).First()),
     };
    await _signInManager.SignInAsync(user, authProperties, CookieAuthenticationDefaults.AuthenticationScheme);

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
      new ClaimsPrincipal(claimsIdentity), authProperties);

    return Ok();
  }

  [HttpPost]
  public async Task<IActionResult> Register(RegisterParameters parameters)
  {
    var user = new Player();
    if (string.IsNullOrEmpty(parameters.UserName)) // this is an anonymous registration
    {
      var stats = (await _statsRepository.ListAsync()).First();
      parameters.UserName = $"user_{stats.PlayersCount + 1}";
    }

    user.UserName = parameters.UserName;
    var result = await _userManager.CreateAsync(user, parameters.Password!);

    if (!result.Succeeded)
    {
      return BadRequest(result.Errors.FirstOrDefault()?.Description);
    }
    
    await _userManager.AddToRoleAsync(user, "Player");

    await _mediator.Publish(new PlayerRegisteredEvent(user));
    
    return await Login(new LoginParameters
    {
      UserName = parameters.UserName, Password = parameters.Password, TokenBasedAuth = parameters.TokenBasedAuth
    });
  }

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> ChangePassword(ChangePasswordParameters parameters)
  {
    var user = await _userManager.FindByNameAsync(parameters.Username!);
    if (user == null) return BadRequest("User does not exist");
    
    var res = await _userManager.ChangePasswordAsync(user, parameters.OldPassword!, parameters.NewPassword!);

    return res.Succeeded ? await Login(new LoginParameters
    {
      UserName = parameters.Username,
      Password = parameters.NewPassword,
      TokenBasedAuth = HttpContext.Request.IsTokenAuth()
    }) : BadRequest(res.Errors);
  }

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    await _signInManager.SignOutAsync();
    return Ok();
  }

  [HttpGet]
  public UserInfo UserInfo()
  {
    //var user = await _userManager.GetUserAsync(HttpContext.User);
    return BuildUserInfo();
  }
  
  private UserInfo BuildUserInfo()
  {
    var userInfo = new UserInfo
    {
      IsAuthenticated = User?.Identity?.IsAuthenticated ?? false,
      UserName = User?.Identity?.Name,
      ExposedClaims = User?.Claims.DistinctBy(x => x.Type).ToDictionary(c => c.Type, c => c.Value)
    };

    return userInfo;
  }

  private string CreateJWT(Player player)
  {
    var secretkey =
      new SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)); // NOTE: SAME KEY AS USED IN Program.cs FILE
    var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

    var claims = new[] // NOTE: could also use List<Claim> here
    {
      new Claim(JwtRegisteredClaimNames.Sub, player.UserName!), // NOTE: this will be the "User.Identity.Name" value
      new Claim("role", "Player"),
      new Claim(JwtRegisteredClaimNames.Jti, player.Id.ToString()) // NOTE: this could a unique ID assigned to the user by a database
    };

    var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"],
      claims: claims, expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("CookieMinutes")),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
