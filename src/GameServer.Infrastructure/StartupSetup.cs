using System.Text;
using GameServer.Core.PlayerAggregate;
using GameServer.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace GameServer.Infrastructure;

public static class StartupSetup
{
  public static void AddDbContext(this IServiceCollection services, ConfigurationManager configuration, string connectionString)
  {
    services.AddDbContext<AppDbContext>(options =>
      options.UseLazyLoadingProxies().UseSqlite(connectionString)); // will be created in web project root
    
    services.AddIdentity<Player, IdentityRole<int>>()
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultUI()
      .AddRoles<IdentityRole<int>>()
      .AddRoleManager<RoleManager<IdentityRole<int>>>()
      .AddDefaultTokenProviders();
    
    var cookieOrJwtScheme = $"CookieSchemeOrJwtScheme";

    services.AddAuthentication(options =>
      {
        options.DefaultScheme = cookieOrJwtScheme;
        options.DefaultChallengeScheme = cookieOrJwtScheme;
      })
      .AddCookie(options =>
      {
        options.LoginPath = "/login";
        options.SlidingExpiration = true;
        options.Cookie.MaxAge = TimeSpan.FromMinutes(configuration.GetValue<int>("CookieMinutes"));
        options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("CookieMinutes"));
      })
      .AddJwtBearer(options =>
      {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidIssuer = configuration["Jwt:Issuer"],
          ValidAudience = configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = false, // false
          ValidateIssuerSigningKey = true
        };
        
        // for signalr jwt authorization
        options.Events = new JwtBearerEvents
        {
          OnMessageReceived = context =>
          {
            var accessToken = StringValues.IsNullOrEmpty(context.Request.Query["access_token"]) ?
              context.Request.Headers["Authorization"] : context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/hubs")))
            {
              // Read the token out of the query string
              context.Token = accessToken;
            }
            return Task.CompletedTask;
          }
        };
      })
      .AddPolicyScheme(cookieOrJwtScheme, cookieOrJwtScheme, options =>
      {
        options.ForwardDefaultSelector = httpContext =>
        {
          string? auth = httpContext.Request.Headers[HeaderNames.Authorization];
          if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
          {
            return JwtBearerDefaults.AuthenticationScheme;
          }

          return CookieAuthenticationDefaults.AuthenticationScheme;
        };
      });

    services.AddAuthorization(options =>
    {
      var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
      defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
      options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
    });

    services.Configure<IdentityOptions>(options =>
    {
      // Password settings
      options.Password.RequireDigit = false;
      options.Password.RequiredLength = 6;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireLowercase = false;

      // Lockout settings
      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
      options.Lockout.MaxFailedAccessAttempts = 10;
      options.Lockout.AllowedForNewUsers = true;

      // User settings
      options.User.RequireUniqueEmail = false;
    });
  }
}
