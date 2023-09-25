namespace GameServer.SharedKernel.ApiModels;

public class MultiplayerGameDTO : BaseDTO
{
  public int GameId { get; set; }
  
  public int EntryCoin { get; set; }
  
  public int PlayerOneId { get; set; }
  
  public int PlayerTwoId { get; set; }
  
  public DateTime? StartTime { get; set; }
  public DateTime? EndTime { get; set; }
  
  public int WinnerPlayerId { get; set; }
}
