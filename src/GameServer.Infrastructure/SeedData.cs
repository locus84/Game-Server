using GameServer.Core.GameAggregate;
using GameServer.Core.MessageAggregate;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.SettingsAggregate;
using GameServer.Core.StatisticsAggregate;
using GameServer.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Infrastructure;

public static class SeedData
{
  public static readonly string[] Roles = new[] { "Admin", "Player", "Viewer" };

  public static async Task Initialize(IServiceProvider serviceProvider)
  {
    await using var dbContext = new AppDbContext(
      serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>(), null);

    // Look for any Players.
    if (dbContext.Users.Any())
    {
      return; // DB has been seeded
    }

    await PopulateRequiredData(serviceProvider.GetRequiredService<UserManager<Player>>()
      , serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>(), dbContext);

    PopulateTestData(dbContext);
  }

  private static async Task PopulateRequiredData(UserManager<Player> userManager,
    RoleManager<IdentityRole<int>> roleManager, AppDbContext dbContext)
  {
    foreach (var role in Roles)
    {
      if (!(await dbContext.Roles.AnyAsync(r => r.Name == role)))
      {
        // var roleStore = new RoleStore<IdentityRole<int>, AppDbContext, int>(dbContext);
        // var res = await roleStore.CreateAsync(new IdentityRole<int>(role));

        var res = await roleManager.CreateAsync(new IdentityRole<int>(role));
      }
    }

    var admin = new Player { UserName = "Admin", Email = "admin@admin.com" };
    if ((await userManager.CreateAsync(admin, "Admin12345")).Succeeded)
    {
      await userManager.AddToRoleAsync(admin, "Admin");
    }
    // ----------------------- 
    await dbContext.AdminSettings.AddAsync(new AdminSettings
    {
      FirebaseKey = "xxxxx-xxxxx-xxxxxx-xxxxx",
      MaxReferralCount = 8,
      EachReferralCoin = 50
    });

    await dbContext.AppSettings.AddAsync(new AppSettings
    {
      PlayerActivationPrice = 12000,
      PlayerActivationDays = 12,
      DeactivationReportsCount = 5,
      BannerRefreshSeconds = 7,
      Chest1EntryCoin = 500000,
      Chest2EntryCoin = 1000000,
      MinPayoutCredit = 100000,
      RandomRefresh = false,
      LtrSlide = false
    });

    await dbContext.Statistics.AddAsync(new Statistics { PlayersCount = 0, OnlinePlayers = 0 });

    await dbContext.Games.AddRangeAsync(new Game[]
    {
      new()
      {
        Name = "Battle Ship",
        TurnBased = true,
        TurnTimeout = 15,
        EntryCoin1 = 200,
        EntryCoin2 = 1000,
        EntryCoin3 = 5000,
        Enabled = true,
      },
      new()
      {
        Name = "Tic Tac Tao",
        TurnBased = true,
        TurnTimeout = 15,
        EntryCoin1 = 200,
        EntryCoin2 = 1000,
        EntryCoin3 = 5000,
        Enabled = true,
      },
      new()
      {
        Name = "Ludo",
        TurnBased = true,
        TurnTimeout = 15,
        EntryCoin1 = 200,
        EntryCoin2 = 1000,
        EntryCoin3 = 5000,
        Enabled = true,
      }
    });

    await dbContext.Inboxes.AddAsync(new Inbox
    {
      Title = "Welcome!",
      Text = "Enjoy online games!"
    });

    await dbContext.ShopItems.AddRangeAsync(new ShopItem[]
    {
      new()
      {
        Title = "",
        Order = 1,
        Sku = "Coins1",
        Coins = 10000,
        Price = 10000,
        Consumable = true,
        Description = "",
      },
      new()
      {
        Title = "",
        Order = 2,
        Sku = "Coins2",
        Coins = 30000,
        Price = 27000,
        Consumable = true,
        Description = "",
      },
      new()
      {
        Title = "",
        Order = 3,
        Sku = "Coins3",
        Coins = 60000,
        Price = 54000,
        Consumable = true,
        Description = "",
      },
      new()
      {
        Title = "",
        Order = 4,
        Sku = "Coins4",
        Coins = 80000,
        Price = 68000,
        Consumable = true,
        Description = "",
      },
      new()
      {
        Title = "",
        Order = 5,
        Sku = "Coins5",
        Coins = 160000,
        Price = 128000,
        Consumable = true,
        Description = "",
      }
    });
  }

  public static void PopulateTestData(AppDbContext dbContext)
  {
    dbContext.SaveChanges();
  }
}
