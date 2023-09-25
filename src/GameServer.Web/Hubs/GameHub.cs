using GameServer.Core.PlayerAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Web.Hubs;

[Authorize]
public class GameHub : InMemoryBaseHub
{
  public const string HubUrl = "/hubs/game";

  public GameHub(ConnectionMapping<string> connectionMapping, GroupMapping<string> groupMapping, IMediator mediator) : 
    base(connectionMapping, groupMapping, mediator) {}
}
