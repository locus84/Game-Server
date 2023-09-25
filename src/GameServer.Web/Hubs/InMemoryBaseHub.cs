using System.Collections.Concurrent;
using Azure.Identity;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.PlayerAggregate.Events;
using GameServer.Core.PlayerAggregate.Specifications;
using GameServer.SharedKernel.Extensions;
using GameServer.SharedKernel.HubModels;
using GameServer.SharedKernel.Interfaces;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Web.Hubs;

public class InMemoryBaseHub : Hub
{
  private readonly ConnectionMapping<string> _connectionMapping;
  private readonly GroupMapping<string> _groupMapping;
  private readonly IMediator _mediator;
  private static readonly ConcurrentDictionary<string, HashSet<MatchMakeRequest>?> _matchMakeRequests = new();

  public InMemoryBaseHub(ConnectionMapping<string> connectionMapping, GroupMapping<string> groupMapping, IMediator mediator)
  {
    _connectionMapping = connectionMapping;
    _groupMapping = groupMapping;
    _mediator = mediator;
  }

  public virtual async Task SendDataAsync(string toUsername, object data, string method = "OnDataReceived")
  {
    var message = new HubMessage
    {
      SenderUsername = Username,
      ReceiverUsername = toUsername,
      Data = data
    };

    foreach (var toConnectionId in _connectionMapping.GetConnections(toUsername))
    {
      await Clients.Client(toConnectionId).SendAsync(method, message);
    }
  }

  public virtual async Task SendToGroupAsync(string groupName, object data, string method = "OnDataReceived")
  {
    var message = new HubMessage
    {
      SenderUsername = Username,
      Data = data
    };

    await Clients.Groups(groupName).SendAsync(method, message);
  }

  public virtual async void BroadcastDataAsync(object data, string method = "OnDataReceived")
  {
    var message = new HubMessage
    {
      SenderUsername = Username,
      Data = data
    };

    await Clients.All.SendAsync(method, message);
  }

  public virtual async Task MatchMakeAsync(MatchMakeRequest matchMakeRequest)
  {
    if (string.IsNullOrEmpty(matchMakeRequest.Username))
      matchMakeRequest.Username = Username;
    
    var matchedRequests = new List<MatchMakeRequest>();
    var requestsToDelete = new List<MatchMakeRequest>();
    
    foreach (var request in _matchMakeRequests.Values.SelectMany(x=>x!).OrderBy(x=> x.RequestTime))
    {
      if (request.RequestTime.Add(TimeSpan.FromMinutes(5)) > DateTime.Now)
      {
        requestsToDelete.Add(request);
        continue;
      }

      if (await matchMakeRequest.Evaluate(request))
      {
        matchedRequests.Add(request);
        
        if (matchMakeRequest.GroupCapacity == matchedRequests.Count + 1)
        {
          matchedRequests.Add(matchMakeRequest);
          
          var matchMakeResp = new MatchMakeResponse
          {
            IsSuccess = true,
            UserNames = matchedRequests.Select(x => x.Username!).ToArray(),
            GroupId = $"Group_{_groupMapping.Count+1}"
          };

          // inform the matched players
          foreach (var matchedRequest in matchedRequests)
          {
            AddToGroup(matchMakeResp.GroupId, matchedRequest.Username);
          }

          await SendToGroupAsync(matchMakeResp.GroupId, matchMakeResp, "OnMatchMakeResult");
          
          break;
        }
      }
    }

    foreach (var request in requestsToDelete)
    {
      _matchMakeRequests.Remove(request.Username!, out HashSet<MatchMakeRequest>? outReq);
    }
  }

  public virtual async void RemoveFromGroup(string groupName, string? username = null)
  {
    username = username ?? Username!;
    foreach (var toConnectionId in _connectionMapping.GetConnections(username))
    {
      await Groups.RemoveFromGroupAsync(toConnectionId, groupName);
    }

    await SendToGroupAsync(groupName, new { Username = username, GroupName = groupName }, "OnLeftTheGroup");
  }

  public virtual async void AddToGroup(string groupName, string? username = null)
  {
    username = username ?? Username!;
    foreach (var toConnectionId in _connectionMapping.GetConnections(username))
    {
      await Groups.AddToGroupAsync(toConnectionId, groupName);
    }

    await SendToGroupAsync(groupName, new { Username = username, GroupName = groupName }, "OnAddedToGroup");
  }

  public override async Task OnConnectedAsync()
  {
    if (!string.IsNullOrEmpty(Username))
    {
      if (!_connectionMapping.HasKey(Username))
      {
        await _mediator.Publish(new PlayerStateChangedEvent(Username, true)).ConfigureAwait(false);
      }
      _connectionMapping.Add(Username, Context.ConnectionId);
    }
    
    await base.OnConnectedAsync();
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    if (!string.IsNullOrEmpty(Username))
    {
      _connectionMapping.Remove(Username, Context.ConnectionId);

      if (!_connectionMapping.HasKey(Username))
      {
        await _mediator.Publish(new PlayerStateChangedEvent(Username, false)).ConfigureAwait(false);
      }
    }
    
    await base.OnDisconnectedAsync(exception);
  }

  protected string? Username => Context.UserIdentifier ?? Context.User?.Identity?.Name;
}
