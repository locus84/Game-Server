namespace GameServer.SharedKernel.ApiModels;

public class PushNotificationDTO : BaseDTO
{
  public string? Title { get; set; }
  public string? Text { get; set; }
  public DateTime? SendTime { get; set; }
}
