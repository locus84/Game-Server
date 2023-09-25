using Ardalis.Result;
using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class MultiplayerGameService : ApiService<MultiplayerGameDTO>, IMultiplayerGameService
{
  public MultiplayerGameService(HttpClient httpClient) : base(httpClient, "api/MultiplayerGames/")
  {
    
  }

  public async Task<Result<ICollection<MultiplayerGameDTO>>> ListByPlayerIdAsync(int playerId)
  {
    return await ListAsync($"ListByPlayerId/{playerId}");
  }
}
