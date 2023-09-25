using Ardalis.Specification;

namespace GameServer.Core.PlayerAggregate.Specifications;

public class PlayerByUsernameSpec : Specification<Player>, ISingleResultSpecification<Player>
{
  public PlayerByUsernameSpec(string username)
  {
    Query.AsNoTracking().Where(x => x.UserName == username);
  }
}
