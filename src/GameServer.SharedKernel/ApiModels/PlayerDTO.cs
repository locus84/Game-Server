namespace GameServer.SharedKernel.ApiModels;

public class PlayerDTO : BaseDTO
{
  public string? Username { get; set; }
  public string? Email { get; set; }
  public string? PhoneNumber { get; set; }
  
  public string? DeviceId { get; set; }
  public string? Name { get; set; }
  public string? DisplayName { get; set; }
  public string? Avatar { get; set; }
  public int Sex { get; set; }
  public string? Province { get; set; }
  public string? City { get; set; }
  public string? DebitCard { get; set; }
  public string? ReferralCode { get; set; }
  public int Coins { get; set; }
  public int Credit { get; set; }
  
  // if reports is greater than AppSettings.ReportsThreshold => set DeactivationTime to now
  public int Reports { get; set; }
  public DateTime? DeactivationTime { get; set; }
  

  public int? ReferredById { get; set; }
}
