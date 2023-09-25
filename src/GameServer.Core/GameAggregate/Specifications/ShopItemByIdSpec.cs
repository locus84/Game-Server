using Ardalis.Specification;

namespace GameServer.Core.GameAggregate.Specifications;

public class ShopItemByIdSpec : Specification<ShopItem>, ISingleResultSpecification<ShopItem>
{
  public ShopItemByIdSpec(int id)
  {
    Query.AsNoTracking().Where(x => x.Id == id);
  }
}
