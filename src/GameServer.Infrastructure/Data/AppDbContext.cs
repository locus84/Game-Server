using System.Reflection;
using GameServer.Core.ChatAggregate;
using GameServer.Core.GameAggregate;
using GameServer.Core.MessageAggregate;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.SettingsAggregate;
using GameServer.Core.StatisticsAggregate;
using GameServer.SharedKernel;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<Player, IdentityRole<int>, int>
{
  private readonly IDomainEventDispatcher? _dispatcher;

  public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher? dispatcher)
    : base(options)
  {
    _dispatcher = dispatcher;
  }

  //public DbSet<Player> Players => Set<Player>();
  public DbSet<Game> Games => Set<Game>();
  public DbSet<MultiplayerGame> MultiplayerGames => Set<MultiplayerGame>();
  public DbSet<Chat> Chats => Set<Chat>();
  public DbSet<Inbox> Inboxes => Set<Inbox>();
  public DbSet<PushNotification> PushNotifications => Set<PushNotification>();
  public DbSet<ShopItem> ShopItems => Set<ShopItem>();
  public DbSet<AdminSettings> AdminSettings => Set<AdminSettings>();
  public DbSet<AppSettings> AppSettings => Set<AppSettings>();
  public DbSet<Statistics> Statistics => Set<Statistics>();
  public DbSet<Chest> Chests => Set<Chest>();
  public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
  public DbSet<Report> Reports => Set<Report>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    builder.Entity<Player>(_ =>
    {
      _.HasMany(f => f.Chats).WithOne(f => f.Player)
        .HasForeignKey(f => f.PlayerId).OnDelete(DeleteBehavior.Cascade);

      _.HasOne(f => f.ReferredBy);
      _.HasMany(f => f.InvitedPlayers).WithOne(f => f.ReferredBy)
        .HasForeignKey(f => f.ReferredById);

      _.HasMany(f => f.Games).WithOne(f => f.PlayerOne)
        .HasForeignKey(f => f.PlayerOneId)
        .OnDelete(DeleteBehavior.Cascade);

      _.HasMany(f => f.FriendRequests).WithOne(f => f.FromPlayer)
        .HasForeignKey(f => f.FromPlayerId);

      _.HasMany(f => f.FriendRequestParents)
        .WithOne(f => f.ToPlayer).HasForeignKey(f => f.ToPlayerId);

      _.HasMany(f => f.ReportedPlayers).WithOne(f => f.Reporter)
        .HasForeignKey(f => f.ReporterId);
      
      _.HasMany(f => f.ReportedPlayerParents).WithOne(f => f.Reported)
        .HasForeignKey(f => f.ReportedId).OnDelete(DeleteBehavior.NoAction);

    });
    builder.Entity<Chat>(_ =>
    {
      _.HasOne(f => f.Receiver);
    });
    builder.Entity<MultiplayerGame>(_ =>
    {
      _.HasOne(f => f.PlayerTwo);
      _.HasOne(f => f.WinnerPlayer);
    });
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<IEntity>()
      .Select(e => e.Entity)
      .Where(e => e.DomainEvents.Any())
      .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
