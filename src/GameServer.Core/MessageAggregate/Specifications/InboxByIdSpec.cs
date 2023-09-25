using Ardalis.Specification;

namespace GameServer.Core.MessageAggregate.Specifications;

public class InboxByIdSpec : Specification<Inbox>, ISingleResultSpecification<Inbox>
{
  public InboxByIdSpec(int id)
  {
    Query.AsNoTracking().Where(x => x.Id == id);
  }
}
