using Ardalis.Specification;

namespace GameServer.Core.GameAggregate.Specifications;

public class MultiplayerGameByIdSpec : Specification<MultiplayerGame>, ISingleResultSpecification<MultiplayerGame>
{
  public MultiplayerGameByIdSpec(int gameId)
  {
    Query.AsNoTracking().Where(x => x.Id == gameId);
  }
}
