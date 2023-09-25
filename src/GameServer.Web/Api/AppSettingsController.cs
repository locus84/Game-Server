using AutoMapper;
using GameServer.Core.SettingsAggregate;
using GameServer.Core.SettingsAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;

public class AppSettingsController : BaseApiController
{
  private readonly IRepository<AppSettings> _repository;
  private readonly IMapper _mapper;

  public AppSettingsController(IRepository<AppSettings> repository, IMapper mapper)
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
      var appSettings = (await _repository.ListAsync()).Select(_mapper.Map<AppSettings, AppSettingsDTO>);
      
      return Ok(appSettings);
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
      var appSettings = await _repository.SingleOrDefaultAsync(new AppSettingsByIdSpec(id));
      if (appSettings == null) return NotFound();

      return Ok(_mapper.Map<AppSettingsDTO>(appSettings));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(AppSettingsDTO appSettingsDto)
  {
    try
    {
      var appSettings = await _repository.SingleOrDefaultAsync(new AppSettingsByIdSpec(appSettingsDto.Id));
      if (appSettings == null) return NotFound();
      
      _mapper.Map(appSettingsDto, appSettings);
      await _repository.UpdateAsync(appSettings);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }

    return Ok();
  }
}
