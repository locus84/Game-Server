using Ardalis.Specification;

namespace GameServer.Core.GameAggregate.Specifications;

public class GameByIdSpec : Specification<Game>, ISingleResultSpecification<Game>
{
  public GameByIdSpec(int gameId)
  {
    Query.AsNoTracking().Where(x => x.Id == gameId);
  }
}
