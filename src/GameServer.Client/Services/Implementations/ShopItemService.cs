using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class ShopItemService : ApiService<ShopItemDTO>, IShopItemService
{
  public ShopItemService(HttpClient httpClient) : base(httpClient, "api/ShopItems/")
  {
  }
}
