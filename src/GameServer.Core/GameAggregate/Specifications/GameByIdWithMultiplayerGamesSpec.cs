using Ardalis.Specification;

namespace GameServer.Core.GameAggregate.Specifications;

public class GameByIdWithMultiplayerGamesSpec : Specification<Game>, ISingleResultSpecification<Game>
{
  public GameByIdWithMultiplayerGamesSpec(int gameId)
  {
    Query.AsNoTracking().Include(x => x.MultiplayerGames).Where(x => x.Id == gameId);
  }
}
