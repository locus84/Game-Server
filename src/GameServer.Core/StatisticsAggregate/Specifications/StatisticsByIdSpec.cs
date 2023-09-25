using Ardalis.Specification;

namespace GameServer.Core.StatisticsAggregate.Specifications;

public class StatisticsByIdSpec : Specification<Statistics>, ISingleResultSpecification<Statistics>
{
  public StatisticsByIdSpec(int id)
  {
    Query.AsNoTracking().Where(x => x.Id == id);
  }
}
