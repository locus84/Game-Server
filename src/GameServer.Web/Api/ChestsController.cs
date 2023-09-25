using AutoMapper;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.PlayerAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;


public class ChestsController : BaseApiController
{
  private readonly IRepository<Chest> _chestRepository;
  private readonly IMapper _mapper;

  public ChestsController(IRepository<Chest> chestRepository, IMapper mapper)
  {
    _chestRepository = chestRepository;
    _mapper = mapper;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var chests = (await _chestRepository.ListAsync()).Select(_mapper.Map<Chest, ChestDTO>);
      
      return Ok(chests);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<IActionResult> Create(ChestDTO chestDto)
  {
    try
    {
      var chest = await _chestRepository.AddAsync(_mapper.Map<Chest>(chestDto));
      
      return Ok(_mapper.Map<ChestDTO>(chest));
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
      var chest = await _chestRepository.FirstOrDefaultAsync(new ChestByIdSpec(id));
      if (chest == null) return NotFound();

      return Ok(_mapper.Map<ChestDTO>(chest));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(ChestDTO chestDto)
  {
    try
    {
      var chest = await _chestRepository.FirstOrDefaultAsync(new ChestByIdSpec(chestDto.Id));
      if (chest == null) return NotFound();

      _mapper.Map(chestDto, chest);
      await _chestRepository.UpdateAsync(chest);
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
      var chest = await _chestRepository.FirstOrDefaultAsync(new ChestByIdSpec(id));
      if (chest == null) return NotFound();

      await _chestRepository.DeleteAsync(chest);

      return Ok(id);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
}
