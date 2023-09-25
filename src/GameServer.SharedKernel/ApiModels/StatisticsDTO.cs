namespace GameServer.SharedKernel.ApiModels;

public class StatisticsDTO : BaseDTO
{
  public int PlayersCount { get; set; }
  public int OnlinePlayers { get; set; }
}
