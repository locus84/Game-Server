using GameServer.Core.PlayerAggregate;
using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.GameAggregate;

public class MultiplayerGame : EntityBase, IAggregateRoot
{
  public virtual Game? Game { get; set; }
  public int GameId { get; set; }
  
  public int EntryCoin { get; set; }
  
  public virtual Player? PlayerOne { get; set; }
  public int PlayerOneId { get; set; }
  
  public virtual Player? PlayerTwo { get; set; }
  public int PlayerTwoId { get; set; }
  
  public DateTime? StartTime { get; set; }
  public DateTime? EndTime { get; set; }
  
  public virtual Player? WinnerPlayer { get; set; }
  public int WinnerPlayerId { get; set; }
    
}
