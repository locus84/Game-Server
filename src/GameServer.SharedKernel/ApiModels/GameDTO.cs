namespace GameServer.SharedKernel.ApiModels;

public class GameDTO : BaseDTO
{
  public string? Name { get; set; }
  public int EntryCoin1 { get; set; }
  public int EntryCoin2 { get; set; }
  public int EntryCoin3 { get; set; }
  public bool TurnBased { get; set; } = true;
  /// <summary>
  /// Turn Timeout in Seconds
  /// </summary>
  public int TurnTimeout { get; set; }
  public bool Enabled { get; set; }
  public bool SearchProvince { get; set; }
  public bool SearchName { get; set; }
  public bool SearchSex { get; set; }
}
