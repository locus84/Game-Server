using Ardalis.Specification;
using GameServer.Core.GameAggregate;

namespace GameServer.Core.PlayerAggregate.Specifications;

public class ChestByIdSpec : Specification<Chest>, ISingleResultSpecification<Chest>
{
  public ChestByIdSpec(int chestId)
  {
    Query.AsNoTracking().Where(x => x.Id == chestId);
  }
}
