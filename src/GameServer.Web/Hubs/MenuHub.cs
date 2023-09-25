using GameServer.Core.PlayerAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Web.Hubs;

[Authorize]
public class MenuHub : InMemoryBaseHub
{
  public const string HubUrl = "/hubs/menu";

  public MenuHub(ConnectionMapping<string> connectionMapping, GroupMapping<string> groupMapping, IMediator mediator) : 
    base(connectionMapping, groupMapping, mediator) {}
}
