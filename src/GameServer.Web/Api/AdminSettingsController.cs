using AutoMapper;
using GameServer.Core.SettingsAggregate;
using GameServer.Core.SettingsAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;

public class AdminSettingsController : BaseApiController
{
  private readonly IRepository<AdminSettings> _repository;
  private readonly IMapper _mapper;

  public AdminSettingsController(IRepository<AdminSettings> repository, IMapper mapper)
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
      var adminSettings = (await _repository.ListAsync()).Select(_mapper.Map<AdminSettings, AdminSettingsDTO>);
      
      return Ok(adminSettings);
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
      var adminSettings = await _repository.SingleOrDefaultAsync(new AdminSettingsByIdSpec(id));
      if (adminSettings == null) return NotFound();

      return Ok(_mapper.Map<AdminSettingsDTO>(adminSettings));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(AdminSettingsDTO adminSettingsDto)
  {
    try
    {
      var adminSettings = await _repository.SingleOrDefaultAsync(new AdminSettingsByIdSpec(adminSettingsDto.Id));
      if (adminSettings == null) return NotFound();

      _mapper.Map(adminSettingsDto, adminSettings);
      await _repository.UpdateAsync(adminSettings);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }

    return Ok();
  }
}
