using AutoMapper;
using GameServer.Core.GameAggregate;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.PlayerAggregate.Specifications;
using GameServer.Infrastructure.Data;
using GameServer.SharedKernel;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Web.Api;

[Authorize]
public class PlayersController : BaseApiController
{
  private readonly IRepository<Player> _playerRepository;
  private readonly IRepository<FriendRequest> _friendRequestRepository;
  private readonly IRepository<Report> _reportRepository;
  private readonly IMapper _mapper;

  public PlayersController(IRepository<Player> playerRepository, IMapper mapper, IRepository<FriendRequest> friendRequestRepository, IRepository<Report> reportRepository)
  {
    _playerRepository = playerRepository;
    _mapper = mapper;
    _friendRequestRepository = friendRequestRepository;
    _reportRepository = reportRepository;
  }

  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var players = (await _playerRepository.ListAsync()).Select(_mapper.Map<Player, PlayerDTO>);

      return Ok(players);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    try
    {
      var player = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(id));
      if (player == null) return NotFound();

      return Ok(_mapper.Map<PlayerDTO>(player));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [HttpPost]
  public async Task<IActionResult> GetByIds([FromBody]int[] ids)
  {
    try
    {
      var players = await _playerRepository.ListAsync(new PlayerByIdsSpec(ids));
      if (players == null) return NotFound();

      return Ok(_mapper.Map<ICollection<PlayerDTO>>(players));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }


  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetFriends(int id)
  {
    try
    {
      var friendRequestsTo = await _friendRequestRepository.ListAsync(new FriendRequestsByIds(toId: id));
      var friendRequestsFrom = await _friendRequestRepository.ListAsync(new FriendRequestsByIds(fromId: id));

      return Ok(_mapper.Map<ICollection<FriendRequestDTO>>(friendRequestsFrom.Concat(friendRequestsTo).Where(x=>x.Accepted)));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }    
  
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetFriendRequests(int id)
  {
    try
    {
      var friendRequests = await _friendRequestRepository.ListAsync(new FriendRequestsByIds(toId: id));
      if (friendRequests == null) return NotFound();

      return Ok(_mapper.Map<ICollection<FriendRequestDTO>>(friendRequests.Where(x=>!x.Accepted)));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }    
  
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetSentFriendRequests(int id)
  {
    try
    {
      var friendRequests = await _friendRequestRepository.ListAsync(new FriendRequestsByIds(fromId: id));
      if (friendRequests == null) return NotFound();

      return Ok(_mapper.Map<ICollection<FriendRequestDTO>>(friendRequests.Where(x=> !x.Accepted)));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }   
  
  [HttpPut("{playerId:int}/{friendId:int}")]
  public async Task<IActionResult> RequestFriendship(int playerId, int friendId)
  {
    try
    {
      var player = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(playerId, nameof(Player.FriendRequests)));
      var friend = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(friendId,  nameof(Player.FriendRequests)));
      if (player == null || friend == null) return NotFound();

      // if (!friend.FriendRequests!.Any(x => x.Id == player.Id))
      // {
      var friendRequest = await _friendRequestRepository.AddAsync(new FriendRequest
      {
        FromPlayerId = playerId, ToPlayerId = friendId, Accepted = false
      });
      
      friend.FriendRequests!.Add(friendRequest);
      await _playerRepository.UpdateAsync(player);
      // }

      return Ok();
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [HttpPut("{playerId:int}/{friendId:int}")]
  public async Task<IActionResult> AcceptFriendship(int playerId, int friendId)
  {
    try
    {
      var friendRequests =
        await _friendRequestRepository.ListAsync(new FriendRequestsByIds(toId: playerId, fromId: friendId));
      if (friendRequests == null || !friendRequests.Any()) return NotFound();

      var friendReq = friendRequests.First();
      friendReq.Accepted = true;
      await _friendRequestRepository.UpdateAsync(friendReq);

      return Ok(_mapper.Map<ICollection<FriendRequestDTO>>(friendRequests));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [HttpDelete("{playerId:int}/{friendId:int}")]
  public async Task<IActionResult> DeleteFriendship(int playerId, int friendId)
  {
    try
    {
      var friendReq = (await _friendRequestRepository.FirstOrDefaultAsync(new FriendRequestsByIds(fromId: playerId, toId: friendId))) ??
                      (await _friendRequestRepository.FirstOrDefaultAsync(new FriendRequestsByIds(toId: playerId, fromId: friendId)));
      if (friendReq == null) return NotFound();
      
      await _friendRequestRepository.DeleteAsync(friendReq);
        
      return Ok();
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
  
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetReportedPlayers(int id)
  {
    try
    {
      var player = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(id, nameof(Player.ReportedPlayers)));
      if (player == null) return NotFound();

      return Ok(_mapper.Map<ICollection<ReportDTO>>(player.ReportedPlayers));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
  
  [HttpPut("{playerId:int}/{reportId:int}")]
  public async Task<IActionResult> ReportPlayer(int playerId, int reportId)
  {
    try
    {
      var player = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(playerId, nameof(Player.ReportedPlayers)));
      var reportedPlayer = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(reportId));
      if (player == null || reportedPlayer == null) return NotFound();
      
      var report = await _reportRepository.AddAsync(new Report
      {
        ReporterId = playerId, ReportedId = reportId
      });
      ++reportedPlayer.Reports;
      player.ReportedPlayers!.Add(report);
      await _playerRepository.UpdateAsync(reportedPlayer);
      await _playerRepository.UpdateAsync(player);

      return Ok();
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }    

  [HttpDelete("{playerId:int}/{reportedId:int}")]
  public async Task<IActionResult> DeleteReportedPlayer(int playerId, int reportedId)
  {
    try
    {
      var report = await _reportRepository.FirstOrDefaultAsync(new ReportByIds(reporterId: playerId, reportedId: reportedId));
      
      if (report == null) return NotFound();
      
      await _reportRepository.DeleteAsync(report);
        
      return Ok();
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
  
  [HttpPost]
  public async Task<IActionResult> GetWithIncludes(GetIncludeParameters parameters)
  {
    try
    {
      var player = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(parameters.Id, parameters.Includes!));
      if (player == null) return NotFound();
  
      return Ok(_mapper.Map<PlayerDTO>(player));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
  
  [HttpPut]
  public async Task<IActionResult> Update(PlayerDTO playerDto)
  {
    try
    {
      var player = await _playerRepository.FirstOrDefaultAsync(new PlayerByIdSpec(playerDto.Id));
      if (player == null) return NotFound();

      _mapper.Map(playerDto, player);

      await _playerRepository.UpdateAsync(player);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }

    return Ok();
  }
}
