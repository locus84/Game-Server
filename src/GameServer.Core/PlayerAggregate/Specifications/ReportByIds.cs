using Ardalis.Specification;

namespace GameServer.Core.PlayerAggregate.Specifications;

public class ReportByIds : Specification<Report>
{
  public ReportByIds(int? reporterId = null, int? reportedId = null)
  {
    if (reporterId.HasValue && reportedId.HasValue)
      Query.Where(x => x.ReportedId == reportedId && x.ReporterId == reporterId);
    else if (reporterId.HasValue)
      Query.Where(x => x.ReporterId == reporterId);
    else
      Query.Where(x => x.ReportedId == reportedId);
  }
}
