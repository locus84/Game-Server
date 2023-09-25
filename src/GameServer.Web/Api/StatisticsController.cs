using AutoMapper;
using GameServer.Core.SettingsAggregate;
using GameServer.Core.SettingsAggregate.Specifications;
using GameServer.Core.StatisticsAggregate;
using GameServer.Core.StatisticsAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;

public class StatisticsController : BaseApiController
{
  private readonly IRepository<Statistics> _repository;
  private readonly IMapper _mapper;

  public StatisticsController(IRepository<Statistics> repository, IMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }
  
  [Authorize]
  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var statistics = (await _repository.ListAsync()).Select(_mapper.Map<Statistics, StatisticsDTO>);
      
      return Ok(statistics);
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
      var statistics = await _repository.SingleOrDefaultAsync(new StatisticsByIdSpec(id));
      if (statistics == null) return NotFound();

      return Ok(_mapper.Map<StatisticsDTO>(statistics));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(StatisticsDTO statisticsDto)
  {
    try
    {
      var statistics = await _repository.SingleOrDefaultAsync(new StatisticsByIdSpec(statisticsDto.Id));
      if (statistics == null) return NotFound();

      _mapper.Map(statisticsDto, statistics);
      await _repository.UpdateAsync(statistics);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }

    return Ok();
  }
}
