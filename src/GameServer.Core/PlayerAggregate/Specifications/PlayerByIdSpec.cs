using System.Linq.Expressions;
using Ardalis.Specification;
using GameServer.Core.GameAggregate;

namespace GameServer.Core.PlayerAggregate.Specifications;

public sealed class PlayerByIdSpec : Specification<Player>, ISingleResultSpecification<Player>
{
  public PlayerByIdSpec(int playerId, params string[] includes)
  {
    var query = Query.AsNoTracking().Where(x => x.Id == playerId);
    foreach (var include in includes)
    {
      query.Include(include);
    }
  }
}

public sealed class PlayerByIdSpec<TProp> : Specification<Player>, ISingleResultSpecification<Player>
{
  public PlayerByIdSpec(int playerId, params Expression<Func<Player, TProp>>[] includes)
  {
    var query = Query.AsNoTracking().Where(x => x.Id == playerId);

      foreach (var include in includes)
      {
        query.Include(include);
      }
  }
}
