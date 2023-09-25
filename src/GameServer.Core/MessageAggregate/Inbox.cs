using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.MessageAggregate;

public class Inbox : EntityBase, IAggregateRoot
{
  public string? Text { get; set; }
  public string? Title { get; set; }
}
