namespace GameServer.SharedKernel.ApiModels;

public class BaseDTO
{
  public int Id { get; set; }

  public DateTime ServerTime => DateTime.UtcNow;
}
