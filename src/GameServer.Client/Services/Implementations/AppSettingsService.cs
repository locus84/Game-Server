
using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class AppSettingsService : ApiService<AppSettingsDTO>, IAppSettingsService
{
  public AppSettingsService(HttpClient httpClient) : base(httpClient, "api/AppSettings/")
  {
  }
}
