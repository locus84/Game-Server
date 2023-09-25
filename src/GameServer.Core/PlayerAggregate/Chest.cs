using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.PlayerAggregate;

public class Chest : EntityBase, IAggregateRoot
{
  public int Reward { get; set; }
  public int Count { get; set; }
  public int Type { get; set; }
}
