using System.Linq.Expressions;
using Ardalis.Specification;
using GameServer.Core.GameAggregate;

namespace GameServer.Core.PlayerAggregate.Specifications;

public sealed class PlayerByIdsSpec : Specification<Player>
{
  public PlayerByIdsSpec(int[] playerIds)
  {
    var query = Query.AsNoTracking().Where(x => playerIds.Any(pId => x.Id == pId));
  }
}
