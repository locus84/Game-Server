using AutoMapper;
using GameServer.Core.MessageAggregate;
using GameServer.Core.MessageAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;

public class InboxesController : BaseApiController
{
   private readonly IRepository<Inbox> _inboxRepository;
  private readonly IMapper _mapper;

  public InboxesController(IRepository<Inbox> inboxRepository, IMapper mapper)
  {
    _inboxRepository = inboxRepository;
    _mapper = mapper;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var inboxDtos = (await _inboxRepository.ListAsync()).Select(_mapper.Map<Inbox, InboxDTO>);
      
      return Ok(inboxDtos);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<IActionResult> Create(InboxDTO gameDto)
  {
    try
    {
      var inbox = await _inboxRepository.AddAsync(_mapper.Map<Inbox>(gameDto));
      
      return Ok(_mapper.Map<InboxDTO>(inbox));
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
      var inbox = await _inboxRepository.FirstOrDefaultAsync(new InboxByIdSpec(id));
      if (inbox == null) return NotFound();

      return Ok(_mapper.Map<GameDTO>(inbox));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(InboxDTO inboxDto)
  {
    try
    {
      var inbox = await _inboxRepository.FirstOrDefaultAsync(new InboxByIdSpec(inboxDto.Id));
      if (inbox == null) return NotFound();

      _mapper.Map(inboxDto, inbox);
      await _inboxRepository.UpdateAsync(inbox);
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
      var inbox = await _inboxRepository.FirstOrDefaultAsync(new InboxByIdSpec(id));
      if (inbox == null) return NotFound();

      await _inboxRepository.DeleteAsync(inbox);

      return Ok(id);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
}
