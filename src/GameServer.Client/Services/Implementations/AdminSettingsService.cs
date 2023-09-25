using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class AdminSettingsService : ApiService<AdminSettingsDTO>, IAdminSettingsService
{
  public AdminSettingsService(HttpClient httpClient) : base(httpClient, "api/AdminSettings/")
  {
  }
}
