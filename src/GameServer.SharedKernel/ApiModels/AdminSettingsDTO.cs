namespace GameServer.SharedKernel.ApiModels;

public class AdminSettingsDTO : BaseDTO
{
  // firebase config
  public string? FirebaseKey { get; set; }
  // dashboard 
  public int? MaxReferralCount { get; set; }
  public int? EachReferralCoin { get; set; }
}
