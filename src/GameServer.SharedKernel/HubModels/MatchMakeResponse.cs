namespace GameServer.SharedKernel.HubModels;

public class MatchMakeResponse
{
  public bool IsSuccess { get; set; } = false;
  public string? GroupId { get; set; }
  public string[]? UserNames { get; set; }
}
