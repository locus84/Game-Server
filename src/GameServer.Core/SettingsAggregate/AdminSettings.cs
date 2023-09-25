using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.Core.SettingsAggregate;

public class AdminSettings : EntityBase, IAggregateRoot
{
  // firebase config
  public string? FirebaseKey { get; set; }
  // dashboard 
  public int? MaxReferralCount { get; set; }
  public int? EachReferralCoin { get; set; }
}
