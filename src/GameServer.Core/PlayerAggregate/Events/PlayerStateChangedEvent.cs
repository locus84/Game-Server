using GameServer.SharedKernel;

namespace GameServer.Core.PlayerAggregate.Events;

public class PlayerStateChangedEvent : DomainEventBase
{
  public string Username { get; }
  public bool IsOnline { get; }

  public PlayerStateChangedEvent(string username, bool isOnline)
  {
    Username = username;
    IsOnline = isOnline;
  }
}
