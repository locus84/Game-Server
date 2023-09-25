namespace GameServer.SharedKernel.ApiModels;

public class FriendRequestDTO : BaseDTO
{
  public int FromPlayerId { get; set; }
  public int ToPlayerId { get; set; }

  public bool Accepted { get; set; }
}
