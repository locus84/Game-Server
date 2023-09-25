using System.ComponentModel.DataAnnotations.Schema;
using GameServer.Core.ChatAggregate;
using GameServer.Core.GameAggregate;
using GameServer.Core.PlayerAggregate.Events;
using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GameServer.Core.PlayerAggregate;

public class Player : IdentityUser<int>, IEntity, IAggregateRoot
{
  public string? DeviceId { get; set; }
  public string? Name { get; set; }
  public string? DisplayName { get; set; }
  public string? Avatar { get; set; }
  public Gender Sex { get; set; }
  public string? Province { get; set; }
  public string? City { get; set; }
  public string? DebitCard { get; set; }
  public string? ReferralCode { get; set; }
  public int Coins { get; set; }
  public int Credit { get; set; }
  public bool IsOnline { get; set; } = false;
  // https://medium.com/@dmitry.pavlov/tree-structure-in-ef-core-how-to-configure-a-self-referencing-table-and-use-it-53effad60bf

  // if reports is greater than AppSettings.ReportsThreshold => set DeactivationTime to now
  public int Reports { get; set; }
  public DateTime? DeactivationTime { get; set; }

  public bool Registered { get; private set; } = false;

  public void MarkRegistered()
  {
    if (!Registered)
    {
      Registered = true;
      
      RegisterDomainEvent(new PlayerRegisteredEvent(this));
    }
  }
  public virtual ICollection<FriendRequest>? FriendRequests { get; set; }
  public virtual ICollection<FriendRequest>? FriendRequestParents { get; set; }

  public virtual ICollection<Report>? ReportedPlayerParents { get; set; }
  public virtual ICollection<Report>? ReportedPlayers { get; set; }
  public virtual Player? ReferredBy { get; set; }
  public int? ReferredById { get; set; }
  public virtual ICollection<Player>? InvitedPlayers { get; set; }

  public virtual ICollection<Chat>? Chats { get; set; }
  public virtual ICollection<MultiplayerGame>? Games { get; set; }

  // ========================================================================
  private List<DomainEventBase> _domainEvents = new();
  [NotMapped]
  public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
  public void ClearDomainEvents() => _domainEvents.Clear();
}
