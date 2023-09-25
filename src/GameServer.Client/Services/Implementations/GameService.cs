using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class GameService : ApiService<GameDTO>, IGameService
{
  public GameService(HttpClient httpClient) : base(httpClient, "api/Games/")
  {
  }
}
