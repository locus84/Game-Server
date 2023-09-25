using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class StatisticsService : ApiService<StatisticsDTO>, IStatisticsService
{
  public StatisticsService(HttpClient httpClient) : base(httpClient, "api/Statistics/")
  {
  }
}
