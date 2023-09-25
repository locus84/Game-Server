using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.MessageAggregate;

public class PushNotification : EntityBase, IAggregateRoot
{
  public string? Title { get; set; }
  public string? Text { get; set; }
  public DateTime? SendTime { get; set; }
}
