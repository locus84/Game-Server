using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.PlayerAggregate;

public class Report : EntityBase, IAggregateRoot
{
  public int ReporterId { get; set; }
  public virtual Player? Reporter { get; set; }
  public int ReportedId { get; set; }
  public virtual Player? Reported { get; set; }
}
