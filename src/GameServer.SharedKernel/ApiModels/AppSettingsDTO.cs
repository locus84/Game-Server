namespace GameServer.SharedKernel.ApiModels;

public class AppSettingsDTO : BaseDTO
{
  // Player Settings
  
  public int PlayerActivationDays { get; set; }
  public int PlayerActivationPrice { get; set; }
  public int DeactivationReportsCount { get; set; }
  
  // Ads Settings

  public int BannerRefreshSeconds { get; set; }
  public bool RandomRefresh { get; set; }
  public bool LtrSlide { get; set; }
  
  public int Chest1EntryCoin { get; set; }
  public int Chest2EntryCoin { get; set; }

  public int MinPayoutCredit { get; set; }
}
