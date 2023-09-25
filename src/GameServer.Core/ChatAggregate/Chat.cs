using GameServer.Core.PlayerAggregate;
using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.ChatAggregate;

public class Chat : EntityBase, IAggregateRoot
{
  /// <summary>
  /// Sender
  /// </summary>
  public virtual Player? Player { get; set; }
  public int PlayerId { get; set; }
  
  public virtual Player? Receiver { get; set; }
  public int ReceiverId { get; set; }
  
  public string? Content { get; set; }
  public DateTime SentTime { get; set; }
}
