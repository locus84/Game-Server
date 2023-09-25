namespace GameServer.SharedKernel.ApiModels;

public class ChestDTO : BaseDTO
{
  public int Reward { get; set; }
  public int Count { get; set; }
  public int Type { get; set; }
}
