using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.PlayerAggregate;

public class FriendRequest : EntityBase, IAggregateRoot
{
  public int FromPlayerId { get; set; }
  public virtual Player? FromPlayer { get; set; }
  public int ToPlayerId { get; set; }
  public virtual Player? ToPlayer { get; set; }
  
  public bool Accepted { get; set; }
}
