namespace GameServer.SharedKernel.ApiModels;

public class ChatDTO : BaseDTO
{
  public virtual PlayerDTO? Player { get; set; }
  public int PlayerId { get; set; }
  
  public virtual PlayerDTO? Receiver { get; set; }
  public int ReceiverId { get; set; }
  
  public string? Content { get; set; }
  public DateTime SentTime { get; set; }
}
