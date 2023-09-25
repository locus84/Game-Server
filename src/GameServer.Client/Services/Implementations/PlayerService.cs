using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class PlayerService : ApiService<PlayerDTO>, IPlayerService
{
  public PlayerService(HttpClient httpClient) : base(httpClient, "api/Players/")
  {
  }
}
