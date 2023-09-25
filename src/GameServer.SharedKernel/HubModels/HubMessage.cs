using MessagePack;

namespace GameServer.SharedKernel.HubModels;

// [MessagePackObject]
public class HubMessage
{
  //[Key(1)]
  public string? SenderUsername { get; set; }
  //[Key(3)]
  public string? ReceiverUsername { get; set; }

  public DateTime ServerTime => DateTime.UtcNow;
  //[Key(4)]
  public object? Data { get; set; }
}
