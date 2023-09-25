using Ardalis.Result;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Contracts;

public interface IMultiplayerGameService : IApiService<MultiplayerGameDTO>
{
  Task<Result<ICollection<MultiplayerGameDTO>>> ListByPlayerIdAsync(int playerId);
}
