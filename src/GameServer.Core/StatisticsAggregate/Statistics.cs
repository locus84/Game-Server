using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.StatisticsAggregate;

public class Statistics : EntityBase, IAggregateRoot
{
  public int PlayersCount { get; set; }
  public int OnlinePlayers { get; set; }
}
