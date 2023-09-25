using Ardalis.Specification;

namespace GameServer.Core.PlayerAggregate.Specifications;

public class PlayerByMobileSpec : Specification<Player>, ISingleResultSpecification<Player>
{
  public PlayerByMobileSpec(string mobile)
  {
    Query.AsNoTracking().Where(x => x.PhoneNumber == mobile);
  }
}
