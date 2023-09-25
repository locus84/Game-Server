using AutoMapper;
using GameServer.Core.GameAggregate;
using GameServer.Core.GameAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;


public class GamesController : BaseApiController
{
  private readonly IRepository<Game> _gameRepository;
  private readonly IMapper _mapper;

  public GamesController(IRepository<Game> gameRepository, IMapper mapper)
  {
    _gameRepository = gameRepository;
    _mapper = mapper;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var games = (await _gameRepository.ListAsync()).Select(_mapper.Map<Game, GameDTO>);
      
      return Ok(games);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<IActionResult> Create(GameDTO gameDto)
  {
    try
    {
      var game = await _gameRepository.AddAsync(_mapper.Map<Game>(gameDto));
      
      return Ok(_mapper.Map<GameDTO>(game));
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
      var game = await _gameRepository.FirstOrDefaultAsync(new GameByIdSpec(id));
      if (game == null) return NotFound();

      return Ok(_mapper.Map<GameDTO>(game));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(GameDTO gameDto)
  {
    try
    {
      var game = await _gameRepository.FirstOrDefaultAsync(new GameByIdSpec(gameDto.Id));
      if (game == null) return NotFound();

      _mapper.Map(gameDto, game);
      await _gameRepository.UpdateAsync(game);
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
      var game = await _gameRepository.FirstOrDefaultAsync(new GameByIdSpec(id));
      if (game == null) return NotFound();

      await _gameRepository.DeleteAsync(game);

      return Ok(id);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
}
