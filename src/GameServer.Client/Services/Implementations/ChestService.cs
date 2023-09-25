using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class ChestService : ApiService<ChestDTO>, IChestService
{
  public ChestService(HttpClient httpClient) : base(httpClient, "api/Chests/")
  {
  }
}
