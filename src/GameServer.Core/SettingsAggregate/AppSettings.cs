using GameServer.Core.PlayerAggregate;
using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.SettingsAggregate;

public class AppSettings : EntityBase, IAggregateRoot
{
  // Player Settings
  
  public int PlayerActivationDays { get; set; } = 12;
  public int PlayerActivationPrice { get; set; }
  public int DeactivationReportsCount { get; set; } = 5;
  
  // Ads Settings

  public int BannerRefreshSeconds { get; set; } = 7;
  public bool RandomRefresh { get; set; } = false;
  public bool LtrSlide { get; set; } = false;
  
  public int Chest1EntryCoin { get; set; }
  public int Chest2EntryCoin { get; set; }

  public int MinPayoutCredit { get; set; }
}
