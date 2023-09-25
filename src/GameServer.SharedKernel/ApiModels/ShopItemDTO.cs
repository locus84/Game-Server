namespace GameServer.SharedKernel.ApiModels;

public class ShopItemDTO : BaseDTO
{
  public int Order { get; set; }
  public string? Sku { get; set; }
  public string? Title { get; set; }
  public int Coins { get; set; }
  public int Price { get; set; }
  public bool Consumable { get; set; }
  public bool IsActive { get; set; }
  public string? Description { get; set; }
}
