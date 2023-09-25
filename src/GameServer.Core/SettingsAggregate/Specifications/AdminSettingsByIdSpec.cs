using Ardalis.Specification;

namespace GameServer.Core.SettingsAggregate.Specifications;

public class AdminSettingsByIdSpec : Specification<AdminSettings>, ISingleResultSpecification<AdminSettings>
{
  public AdminSettingsByIdSpec(int id)
  {
    Query.AsNoTracking().Where(x => x.Id == id);
  }
}
