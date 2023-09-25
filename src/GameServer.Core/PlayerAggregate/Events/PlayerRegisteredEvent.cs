using GameServer.SharedKernel;

namespace GameServer.Core.PlayerAggregate.Events;

public class PlayerRegisteredEvent : DomainEventBase
{
  public Player Player { get; private set; }

  public PlayerRegisteredEvent(Player player)
  {
    Player = player;
  }
}
