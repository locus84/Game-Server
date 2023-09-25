using AutoMapper;
using GameServer.Core.GameAggregate;
using GameServer.Core.GameAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;


public class MultiplayerGamesController : BaseApiController
{
  private readonly IRepository<MultiplayerGame> _multiplayerGameRepository;
  private readonly IMapper _mapper;

  public MultiplayerGamesController(IRepository<MultiplayerGame> multiplayerGameRepository, IMapper mapper)
  {
    _multiplayerGameRepository = multiplayerGameRepository;
    _mapper = mapper;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var games = (await _multiplayerGameRepository.ListAsync()).Select(_mapper.Map<MultiplayerGame, MultiplayerGameDTO>);
      
      return Ok(games);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
  
  [Authorize]
  [HttpGet("{playerId:int}")]
  public async Task<IActionResult> ListByPlayerId(int playerId)
  {
    try
    {
      var games = (await _multiplayerGameRepository.ListAsync(new MultiplayerGamesByPlayerIdSpec(playerId)))
        .Select(_mapper.Map<MultiplayerGame, MultiplayerGameDTO>);
      
      return Ok(games);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<IActionResult> Create(MultiplayerGameDTO gameDto)
  {
    try
    {
      var game = await _multiplayerGameRepository.AddAsync(_mapper.Map<MultiplayerGame>(gameDto));
      
      return Ok(_mapper.Map<MultiplayerGameDTO>(game));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize]
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    try
    {
      var game = await _multiplayerGameRepository.FirstOrDefaultAsync(new MultiplayerGameByIdSpec(id));
      if (game == null) return NotFound();

      return Ok(_mapper.Map<MultiplayerGameDTO>(game));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(MultiplayerGameDTO gameDto)
  {
    try
    {
      var game = await _multiplayerGameRepository.FirstOrDefaultAsync(new MultiplayerGameByIdSpec(gameDto.Id));
      if (game == null) return NotFound();

      _mapper.Map(gameDto, game);
      await _multiplayerGameRepository.UpdateAsync(game);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }

    return Ok();
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      var game = await _multiplayerGameRepository.FirstOrDefaultAsync(new MultiplayerGameByIdSpec(id));
      if (game == null) return NotFound();

      await _multiplayerGameRepository.DeleteAsync(game);

      return Ok(id);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
}
