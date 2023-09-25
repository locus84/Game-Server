using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.GameAggregate;

public class ShopItem : EntityBase, IAggregateRoot
{
  public int Order { get; set; }
  public string? Sku { get; set; }
  public string? Title { get; set; } = "Coin Pack 1";
  public int Coins { get; set; }
  public int Price { get; set; }
  public bool Consumable { get; set; } = true;
  public bool IsActive { get; set; } = true;
  public string? Description { get; set; } = "Coin Pack 1";
}
