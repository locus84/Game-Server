using Ardalis.Specification;

namespace GameServer.Core.SettingsAggregate.Specifications;

public class AppSettingsByIdSpec : Specification<AppSettings>, ISingleResultSpecification<AppSettings>
{
  public AppSettingsByIdSpec(int id)
  {
    Query.AsNoTracking().Where(x => x.Id == id);
  }
}
