using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsPlatform.Infrastructure.Seed;

public static class SeedData
{
    private static readonly string[] IdentityRoles = ["User", "Admin"];

    private const string TestUserName = "testuser";
    private const string TestUserEmail = "testuser@logistics.local";
    private const string TestUserPassword = "Test123!";
    private const string TestUserIdentityRole = "User";

    private static readonly Guid User1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid User2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid User3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid User5Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid HubMarkhamId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    private static readonly Guid HubTorontoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

    private static readonly Guid CarrierDriver5Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
    private static readonly Guid CarrierSchneiderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
    private static readonly Guid CarrierTForceId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
    private static readonly Guid CarrierSelfId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4");

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();

        RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (string roleName in IdentityRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        UserManager<ApplicationUser> userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        await EnsureUserAsync(
            userManager,
            User1Id,
            TestUserName,
            TestUserEmail,
            "User 1",
            "U1",
            UserRole.Admin,
            balanceCents: 100,
            password: TestUserPassword,
            identityRole: TestUserIdentityRole);

        await EnsureUserAsync(
            userManager,
            User2Id,
            "user2",
            "user2@logistics.local",
            "User 2",
            "U2",
            UserRole.Dispatcher,
            balanceCents: 0,
            password: TestUserPassword,
            identityRole: "User");

        await EnsureUserAsync(
            userManager,
            User3Id,
            "user3",
            "user3@logistics.local",
            "User 3",
            "U3",
            UserRole.Dispatcher,
            balanceCents: 0,
            password: TestUserPassword,
            identityRole: "User");

        await EnsureUserAsync(
            userManager,
            User5Id,
            "user5",
            "user5@logistics.local",
            "User 5",
            "U5",
            UserRole.Driver,
            balanceCents: 0,
            password: TestUserPassword,
            identityRole: "User");

        await SeedCatalogAndOrdersAsync(dbContext);
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        Guid id,
        string userName,
        string email,
        string displayName,
        string initials,
        UserRole role,
        long balanceCents,
        string password,
        string identityRole)
    {
        ApplicationUser? byId = await userManager.FindByIdAsync(id.ToString());
        if (byId is not null)
        {
            await UpdateSeedUserAsync(userManager, byId, displayName, initials, role, balanceCents, identityRole);
            return;
        }

        ApplicationUser? byName = await userManager.FindByNameAsync(userName);
        if (byName is not null)
            await userManager.DeleteAsync(byName);

        ApplicationUser? byEmail = await userManager.FindByEmailAsync(email);
        if (byEmail is not null && byEmail.Id != id)
            await userManager.DeleteAsync(byEmail);

        var user = new ApplicationUser
        {
            Id = id,
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            ExternalId = id.ToString("N"),
            CreatedAt = DateTime.UtcNow,
            DisplayName = displayName,
            Initials = initials,
            Role = role,
            BalanceCents = balanceCents,
            IsActive = true
        };

        IdentityResult createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed user {userName}: {errors}");
        }

        await userManager.AddToRoleAsync(user, identityRole);
    }

    private static async Task UpdateSeedUserAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string displayName,
        string initials,
        UserRole role,
        long balanceCents,
        string identityRole)
    {
        user.DisplayName = displayName;
        user.Initials = initials;
        user.Role = role;
        user.BalanceCents = balanceCents;
        user.IsActive = true;

        IdentityResult updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update seed user {user.UserName}: {errors}");
        }

        if (!await userManager.IsInRoleAsync(user, identityRole))
            await userManager.AddToRoleAsync(user, identityRole);
    }

    private static async Task SeedCatalogAndOrdersAsync(AppDbContext dbContext)
    {
        if (!await dbContext.Hubs.AnyAsync())
        {
            dbContext.Hubs.AddRange(
                new Hub { Id = HubMarkhamId, Name = "Markham", RegionCode = "ON" },
                new Hub { Id = HubTorontoId, Name = "Toronto", RegionCode = "ON" });
        }

        if (!await dbContext.Carriers.AnyAsync())
        {
            dbContext.Carriers.AddRange(
                new Carrier { Id = CarrierDriver5Id, Name = "Driver: User 5", DriverUserId = User5Id },
                new Carrier { Id = CarrierSchneiderId, Name = "Schneider" },
                new Carrier { Id = CarrierTForceId, Name = "TForce" },
                new Carrier { Id = CarrierSelfId, Name = "Self pickup" });
        }

        await dbContext.SaveChangesAsync();

        if (await dbContext.Orders.AnyAsync())
            return;

        var now = DateTimeOffset.UtcNow;

        var order676 = new Order
        {
            Id = Guid.Parse("c0000000-0000-0000-0000-000000001676"),
            Number = "FR001676",
            Type = OrderType.Consolidation,
            Status = OrderStatus.InProgress,
            HubId = HubMarkhamId,
            ScheduledAt = new DateTimeOffset(2026, 4, 12, 9, 0, 0, TimeSpan.Zero),
            DestinationCity = "Toronto",
            DestinationRegion = "ON",
            CreatedByUserId = User1Id,
            CarrierId = CarrierDriver5Id,
            TrailersConsolidated = 2,
            NextActionKind = NextActionKind.Loading,
            NextActionLabel = "Loading",
            NextActionDueAt = now.AddHours(2).AddMinutes(14),
            CreatedAt = now.AddDays(-10),
            QuantityLines =
            {
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 15 },
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.XL, Count = 3 }
            },
            SubOrders =
            {
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001676-1", Reference = "REF-1001", PalletCount = 9, SortOrder = 1 },
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001676-2", Reference = "REF-1003", PalletCount = 6, SortOrder = 2 },
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001676-3", Reference = "REF-1002", PalletCount = 12, SortOrder = 3 }
            }
        };

        var order681 = new Order
        {
            Id = Guid.Parse("c0000000-0000-0000-0000-000000001681"),
            Number = "FR001681",
            Type = OrderType.CrossDock,
            Status = OrderStatus.New,
            HubId = HubTorontoId,
            ScheduledAt = new DateTimeOffset(2026, 4, 15, 14, 0, 0, TimeSpan.Zero),
            DestinationCity = "Detroit",
            DestinationRegion = "MI",
            DestinationNote = "via External PDF",
            CreatedByUserId = User2Id,
            CarrierId = CarrierSchneiderId,
            AwaitingClientAction = true,
            NextActionKind = NextActionKind.WaitingForTruck,
            NextActionLabel = "Waiting for truck",
            CreatedAt = now.AddDays(-3),
            QuantityLines =
            {
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 23 }
            },
            SubOrders =
            {
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001681-1", Reference = "REF-1004", PalletCount = 23, SortOrder = 1 }
            }
        };

        var order674 = new Order
        {
            Id = Guid.Parse("c0000000-0000-0000-0000-000000001674"),
            Number = "FR001674",
            Type = OrderType.Consolidation,
            Status = OrderStatus.Alert,
            HubId = HubMarkhamId,
            ScheduledAt = new DateTimeOffset(2026, 4, 13, 11, 0, 0, TimeSpan.Zero),
            DestinationCity = "Calgary",
            DestinationRegion = "AB",
            CreatedByUserId = User3Id,
            CarrierId = CarrierTForceId,
            DeclaredQty = 20,
            ActualQty = 18,
            TrailersConsolidated = 1,
            AwaitingClientAction = true,
            HasAlert = true,
            AlertReason = "photo_missing",
            NextActionKind = NextActionKind.UploadPhoto,
            NextActionLabel = "Upload photo",
            CreatedAt = now.AddDays(-8),
            SubOrders =
            {
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001674-1", Reference = "REF-1005", PalletCount = 11, SortOrder = 1 },
                new SubOrder
                {
                    Id = Guid.NewGuid(),
                    Number = "FR001674-2",
                    Reference = "REF-1006",
                    PalletCount = 7,
                    HasMissingPhoto = true,
                    SortOrder = 2
                }
            }
        };

        var order672 = new Order
        {
            Id = Guid.Parse("c0000000-0000-0000-0000-000000001672"),
            Number = "FR001672",
            Type = OrderType.CrossDock,
            Status = OrderStatus.Completed,
            HubId = HubMarkhamId,
            ScheduledAt = new DateTimeOffset(2026, 4, 14, 17, 30, 0, TimeSpan.Zero),
            DestinationCity = "Brampton",
            DestinationRegion = "ON",
            DestinationNote = "Order with photos",
            CreatedByUserId = User1Id,
            CarrierId = CarrierSelfId,
            NextActionKind = NextActionKind.Paid,
            NextActionLabel = "Paid",
            NextActionAmountCents = 100,
            NextActionDocumentNumber = "D01812",
            CreatedAt = now.AddDays(-20),
            CompletedAt = now.AddDays(-5),
            SpendCents = 100,
            QuantityLines =
            {
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.XL, Count = 10 }
            },
            SubOrders =
            {
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001672-1", Reference = "REF-1007", PalletCount = 10, SortOrder = 1 }
            }
        };

        dbContext.Orders.AddRange(order676, order681, order674, order672);

        for (var i = 0; i < 4; i++)
        {
            dbContext.Orders.Add(new Order
            {
                Id = Guid.Parse($"c0000000-0000-0000-0000-00000001000{i}"),
                Number = $"FR00169{i}",
                Type = OrderType.CrossDock,
                Status = OrderStatus.InProgress,
                HubId = HubMarkhamId,
                ScheduledAt = now.AddDays(i),
                DestinationCity = "Toronto",
                DestinationRegion = "ON",
                CreatedByUserId = User1Id,
                CarrierId = CarrierTForceId,
                CreatedAt = now.AddDays(-i),
                NextActionLabel = "Loading",
                NextActionKind = NextActionKind.Loading,
                QuantityLines =
                {
                    new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 5 + i }
                },
                SubOrders =
                {
                    new SubOrder
                    {
                        Id = Guid.NewGuid(),
                        Number = $"FR00169{i}-1",
                        Reference = $"REF-11{i:00}",
                        PalletCount = 5 + i,
                        SortOrder = 1
                    }
                }
            });
        }

        dbContext.Orders.Add(new Order
        {
            Id = Guid.Parse("c0000000-0000-0000-0000-000000001690"),
            Number = "FR001690",
            Type = OrderType.CrossDock,
            Status = OrderStatus.New,
            HubId = HubTorontoId,
            ScheduledAt = now.AddDays(2),
            DestinationCity = "Ottawa",
            DestinationRegion = "ON",
            CreatedByUserId = User2Id,
            CarrierId = CarrierSchneiderId,
            AwaitingClientAction = true,
            CreatedAt = now.AddDays(-1),
            NextActionLabel = "Confirm details",
            QuantityLines =
            {
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 8 }
            },
            SubOrders =
            {
                new SubOrder
                {
                    Id = Guid.NewGuid(),
                    Number = "FR001690-1",
                    Reference = "REF-1200",
                    PalletCount = 8,
                    SortOrder = 1
                }
            }
        });

        for (var w = 0; w < 10; w++)
        {
            var completedAt = now.AddDays(-(9 - w) * 7);
            var count = 1 + w / 2;
            for (var n = 0; n < count; n++)
            {
                dbContext.Orders.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    Number = $"FR00{2000 + w * 10 + n}",
                    Type = OrderType.CrossDock,
                    Status = OrderStatus.Completed,
                    HubId = HubMarkhamId,
                    ScheduledAt = completedAt.AddDays(-1),
                    DestinationCity = "Toronto",
                    DestinationRegion = "ON",
                    CreatedByUserId = User1Id,
                    CarrierId = CarrierSelfId,
                    CreatedAt = completedAt.AddDays(-3),
                    CompletedAt = completedAt,
                    SpendCents = w == 6 ? 100 : 0,
                    NextActionKind = NextActionKind.Paid,
                    NextActionLabel = "Paid",
                    NextActionAmountCents = w == 6 ? 100 : 0,
                    QuantityLines =
                    {
                        new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 4 }
                    },
                    SubOrders =
                    {
                        new SubOrder
                        {
                            Id = Guid.NewGuid(),
                            Number = $"FR00{2000 + w * 10 + n}-1",
                            Reference = $"REF-{2000 + w * 10 + n}",
                            PalletCount = 4,
                            SortOrder = 1
                        }
                    }
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}