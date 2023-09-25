using Ardalis.Specification;
using GameServer.Core.PlayerAggregate;

namespace GameServer.Core.GameAggregate.Specifications;

public class MultiplayerGamesByPlayerIdSpec : Specification<MultiplayerGame>
{
  public MultiplayerGamesByPlayerIdSpec(int playerId)
  {
    Query.AsNoTracking().Where(x => x.PlayerOneId == playerId || x.PlayerTwoId == playerId);
  }
}
