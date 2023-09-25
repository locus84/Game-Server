using Ardalis.GuardClauses;
using GameServer.Core.PlayerAggregate.Events;
using GameServer.Core.PlayerAggregate.Specifications;
using GameServer.Core.StatisticsAggregate;
using GameServer.SharedKernel.Interfaces;
using MediatR;

namespace GameServer.Core.PlayerAggregate.Handlers;

public class PlayerStateChangedHandler : INotificationHandler<PlayerStateChangedEvent>
{
  private readonly IRepository<Statistics> _statsRepo;
  private readonly IRepository<Player> _playerRepo;

  public PlayerStateChangedHandler(IRepository<Statistics> statsRepo, IRepository<Player> playerRepo)
  {
    _statsRepo = statsRepo;
    _playerRepo = playerRepo;
  }
  
  public async Task Handle(PlayerStateChangedEvent notification, CancellationToken cancellationToken)
  {
    var player = await _playerRepo.FirstOrDefaultAsync(new PlayerByUsernameSpec(notification.Username), cancellationToken);
    if (player != null && player.IsOnline != notification.IsOnline)
    {
      var stats = (await _statsRepo.ListAsync(cancellationToken)).FirstOrDefault();
      Guard.Against.Null(stats);
      
      stats.OnlinePlayers += notification.IsOnline ? 1 : -1;

      await _statsRepo.UpdateAsync(stats, cancellationToken);
    }
  }
}
