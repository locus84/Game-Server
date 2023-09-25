using GameServer.Core.PlayerAggregate.Events;
using GameServer.Core.StatisticsAggregate;
using GameServer.SharedKernel.Interfaces;
using MediatR;

namespace GameServer.Core.PlayerAggregate.Handlers;

public class PlayerRegisteredHandler : INotificationHandler<PlayerRegisteredEvent>
{
  private readonly IRepository<Statistics> _statsRepo;

  public PlayerRegisteredHandler(IRepository<Statistics> statsRepo)
  {
    _statsRepo = statsRepo;
  }

  public async Task Handle(PlayerRegisteredEvent notification, CancellationToken cancellationToken)
  {
    var stats = (await _statsRepo.ListAsync(cancellationToken))?.FirstOrDefault();
    if (stats != null)
    {
      ++stats.PlayersCount;

      await _statsRepo.UpdateAsync(stats, cancellationToken);
    }
  }
}
