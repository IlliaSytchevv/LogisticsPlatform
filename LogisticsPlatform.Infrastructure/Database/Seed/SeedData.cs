using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Services;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsPlatform.Infrastructure.Database.Seed;

public static class SeedData
{
    private static readonly string[] IdentityRoles = ["Admin", "Dispatcher", "Driver"];
    private const string Password = "Test123!";

    private static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid DispatcherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid DriverUserId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid HubMarkhamId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    private static readonly Guid HubTorontoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

    private static readonly Guid DockD1Id = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid DockD2Id = Guid.Parse("a1000000-0000-0000-0000-000000000002");
    private static readonly Guid DockT1Id = Guid.Parse("a1000000-0000-0000-0000-000000000003");

    private static readonly Guid CarrierDriver5Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
    private static readonly Guid CarrierSchneiderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
    private static readonly Guid CarrierTForceId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
    private static readonly Guid CarrierSelfId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4");

    private static readonly Guid Order676Id = Guid.Parse("c0000000-0000-0000-0000-000000001676");
    private static readonly Guid Order681Id = Guid.Parse("c0000000-0000-0000-0000-000000001681");
    private static readonly Guid Order674Id = Guid.Parse("c0000000-0000-0000-0000-000000001674");
    private static readonly Guid Order672Id = Guid.Parse("c0000000-0000-0000-0000-000000001672");
    private static readonly Guid DraftOrderId = Guid.Parse("c0000000-0000-0000-0000-000000000101");
    private static readonly Guid ClosedOrderId = Guid.Parse("c0000000-0000-0000-0000-000000000102");

    private static readonly Guid Op676LoadingId = Guid.Parse("a2000000-0000-0000-0000-000000000001");
    private static readonly Guid Op676UnloadingId = Guid.Parse("a2000000-0000-0000-0000-000000000002");

    private static readonly byte[] TinyPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        await EnsureRolesAsync(scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>());
        await EnsureUsersAsync(scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>());

        if (await db.Orders.AnyAsync())
            return;

        IPhotoBlobStore photos = scope.ServiceProvider.GetRequiredService<IPhotoBlobStore>();
        DateTimeOffset now = DateTimeOffset.UtcNow;

        await SeedReferenceDataAsync(db);
        await SeedCatalogAsync(db);
        await SeedOrdersAsync(db, photos, now);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (string roleName in IdentityRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    private static async Task EnsureUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await EnsureUserAsync(userManager, AdminUserId, "AdminUser", "Adminuser@logistics.local", "Admin User", "AU", UserRole.Admin, 100, "Admin");
        await EnsureUserAsync(userManager, DispatcherUserId, "DispatcherUser", "DispatcherUser@logistics.local", "Dispatcher User", "DU", UserRole.Dispatcher, 0, "Dispatcher");
        await EnsureUserAsync(userManager, DriverUserId, "DriverUser", "DriverUser@logistics.local", "Driver User", "DR", UserRole.Driver, 0, "Driver");
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
        string identityRole)
    {
        ApplicationUser? byId = await userManager.FindByIdAsync(id.ToString());
        if (byId is not null)
        {
            byId.DisplayName = displayName;
            byId.Initials = initials;
            byId.Role = role;
            byId.BalanceCents = balanceCents;
            byId.IsActive = true;
            await userManager.UpdateAsync(byId);

            if (!await userManager.IsInRoleAsync(byId, identityRole))
                await userManager.AddToRoleAsync(byId, identityRole);

            foreach (string existing in await userManager.GetRolesAsync(byId))
            {
                if (!string.Equals(existing, identityRole, StringComparison.OrdinalIgnoreCase))
                    await userManager.RemoveFromRoleAsync(byId, existing);
            }

            return;
        }

        ApplicationUser? byName = await userManager.FindByNameAsync(userName);
        if (byName is not null)
            await userManager.DeleteAsync(byName);

        ApplicationUser? byEmail = await userManager.FindByEmailAsync(email);
        if (byEmail is not null)
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

        IdentityResult create = await userManager.CreateAsync(user, Password);
        if (!create.Succeeded)
            throw new InvalidOperationException($"Seed user {userName}: {string.Join(", ", create.Errors.Select(e => e.Description))}");

        await userManager.AddToRoleAsync(user, identityRole);
    }

    private static async Task SeedReferenceDataAsync(AppDbContext db)
    {
        db.Hubs.AddRange(
            new Hub { Id = HubMarkhamId, Name = "Markham", RegionCode = "ON" },
            new Hub { Id = HubTorontoId, Name = "Toronto", RegionCode = "ON" });

        db.HubDocks.AddRange(
            new HubDock { Id = DockD1Id, HubId = HubMarkhamId, Code = "D1", BayLabel = "Bay A", SortOrder = 1 },
            new HubDock { Id = DockD2Id, HubId = HubMarkhamId, Code = "D2", BayLabel = "Bay B", SortOrder = 2 },
            new HubDock { Id = DockT1Id, HubId = HubTorontoId, Code = "T1", BayLabel = "Bay 1", SortOrder = 1 });

        db.Carriers.AddRange(
            new Carrier { Id = CarrierDriver5Id, Name = "Driver: Driver User", DriverUserId = DriverUserId },
            new Carrier { Id = CarrierSchneiderId, Name = "Schneider" },
            new Carrier { Id = CarrierTForceId, Name = "TForce" },
            new Carrier { Id = CarrierSelfId, Name = "Self pickup" });

        await db.SaveChangesAsync();
    }

    private static async Task SeedCatalogAsync(AppDbContext db)
    {
        db.SupplyCatalogItems.AddRange(
            Catalog(1, "WRAP-001", "Shrink wrap 120g", "Packaging", 120, 70, 20),
            Catalog(2, "WRAP-002", "Stretch film 500m", "Packaging", 180, 110, 18),
            Catalog(3, "STRAP-12", "Straps 12'", "Securing", 80, 45, 22),
            Catalog(4, "STRAP-18", "Straps 18'", "Securing", 95, 55, 22),
            Catalog(5, "CORN-50", "Corners 50", "Protection", 50, 28, 25),
            Catalog(6, "CORN-100", "Corners 100", "Protection", 90, 52, 25),
            Catalog(7, "PAL-STD", "Standard pallet", "Pallets", 1500, 900, 15),
            Catalog(8, "PAL-XL", "XL pallet", "Pallets", 2200, 1400, 15),
            Catalog(9, "LABEL-A4", "Shipping labels A4 (100)", "Labels", 40, 22, 30),
            Catalog(10, "TAPE-CLR", "Clear packing tape", "Packaging", 35, 18, 28),
            Catalog(11, "EDGE-BD", "Edge boards (pair)", "Protection", 110, 65, 20),
            Catalog(12, "BAND-PET", "PET banding roll", "Securing", 250, 160, 18),
            Catalog(13, "SLIP-SHT", "Slip sheet", "Pallets", 75, 40, 24),
            Catalog(14, "DUNN-BAG", "Dunnage air bags", "Protection", 320, 200, 16),
            Catalog(15, "SEAL-MET", "Metal seals (10)", "Securing", 60, 30, 30),
            Catalog(16, "GLOVE-M", "Work gloves M", "Safety", 45, 22, 35));

        await db.SaveChangesAsync();
    }

    private static SupplyCatalogItem Catalog(
        int sort, string sku, string name, string category,
        long platform, long wholesale, decimal margin) =>
        new()
        {
            Id = Guid.Parse($"d1000000-0000-0000-0000-0000000000{sort:D2}"),
            Sku = sku,
            Name = name,
            Category = category,
            PlatformPriceCents = platform,
            WholesalePriceCents = wholesale,
            MarginSplitPercent = margin,
            SortOrder = sort,
            IsActive = true
        };

    private static async Task SeedOrdersAsync(AppDbContext db, IPhotoBlobStore photos, DateTimeOffset now)
    {
        var order676 = new Order
        {
            Id = Order676Id,
            Number = "FR001676",
            Type = OrderType.Consolidation,
            Status = OrderStatus.InProgress,
            HubId = HubMarkhamId,
            ScheduledAt = new DateTimeOffset(2026, 4, 12, 9, 0, 0, TimeSpan.Zero),
            DestinationCity = "Toronto",
            DestinationRegion = "ON",
            CreatedByUserId = AdminUserId,
            CarrierId = CarrierDriver5Id,
            DeclaredQty = 18,
            ActualQty = 18,
            TrailersConsolidated = 2,
            CreatedAt = now.AddDays(-10),
            NextAction = new OrderNextAction
            {
                NextActionKind = NextActionKind.Loading,
                NextActionLabel = "Loading",
                NextActionDueAt = now.AddHours(2)
            },
            Cabinet = new OrderCabinetDetail
            {
                CustomerName = "Acme Logistics Inc.",
                PrimaryReference = "REF-1001",
                Phone = "+1 416 555 0101",
                TrailerType = "Van · 53ft",
                TruckNumber = "TRK-1201",
                TrailerNumber = "TRL-8801",
                ServicesCsv = "Transload,Restack",
                QuantityUnitLabel = "Pallets",
                StockStatusLabel = "Counted",
                LoadingStatusLabel = "In progress"
            },
            Dock = new OrderDockAssignment
            {
                DockCode = "D1",
                DockBay = "Bay A",
                DockAssignedAt = now.AddDays(-1),
                DockStatusLabel = "Trailer docked · loading",
                AssignedToUserId = DriverUserId,
                WarehouseNote = "Counted 18 pallets on arrival."
            },
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
            Id = Order681Id,
            Number = "FR001681",
            Type = OrderType.CrossDock,
            Status = OrderStatus.New,
            HubId = HubTorontoId,
            ScheduledAt = new DateTimeOffset(2026, 4, 15, 14, 0, 0, TimeSpan.Zero),
            DestinationCity = "Detroit",
            DestinationRegion = "MI",
            DestinationNote = "via External PDF",
            CreatedByUserId = DispatcherUserId,
            CarrierId = CarrierSchneiderId,
            DeclaredQty = 23,
            CreatedAt = now.AddDays(-3),
            NextAction = new OrderNextAction
            {
                AwaitingClientAction = true,
                NextActionKind = NextActionKind.WaitingForTruck,
                NextActionLabel = "Waiting for truck"
            },
            Cabinet = new OrderCabinetDetail
            {
                CustomerName = "Northern Freight Co.",
                PrimaryReference = "REF-1004",
                TrailerType = "53' Dry Van",
                ServicesCsv = "Cross-dock,Pallet wrap"
            },
            Dock = new OrderDockAssignment
            {
                DockCode = "T1",
                DockBay = "Bay 1",
                DockAssignedAt = now.AddHours(-6),
                DockStatusLabel = "Assigned",
                AssignedToUserId = DispatcherUserId
            },
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
            Id = Order674Id,
            Number = "FR001674",
            Type = OrderType.Consolidation,
            Status = OrderStatus.Alert,
            HubId = HubMarkhamId,
            ScheduledAt = new DateTimeOffset(2026, 4, 13, 11, 0, 0, TimeSpan.Zero),
            DestinationCity = "Calgary",
            DestinationRegion = "AB",
            CreatedByUserId = DispatcherUserId,
            CarrierId = CarrierTForceId,
            DeclaredQty = 20,
            ActualQty = 18,
            TrailersConsolidated = 1,
            HasAlert = true,
            AlertReason = "photo_missing",
            CreatedAt = now.AddDays(-8),
            NextAction = new OrderNextAction
            {
                AwaitingClientAction = true,
                NextActionKind = NextActionKind.UploadPhoto,
                NextActionLabel = "Upload photo"
            },
            Cabinet = new OrderCabinetDetail
            {
                CustomerName = "Brampton Retail Hub",
                PrimaryReference = "REF-1005",
                ServicesCsv = "Consolidation,Photo check"
            },
            Dock = new OrderDockAssignment
            {
                DockCode = "D2",
                DockBay = "Bay B",
                DockStatusLabel = "Waiting",
                AssignedToUserId = DispatcherUserId,
                WarehouseNote = "Missing photo on REF-1006."
            },
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
            Id = Order672Id,
            Number = "FR001672",
            Type = OrderType.CrossDock,
            Status = OrderStatus.Completed,
            HubId = HubMarkhamId,
            ScheduledAt = new DateTimeOffset(2026, 4, 14, 17, 30, 0, TimeSpan.Zero),
            DestinationCity = "Brampton",
            DestinationRegion = "ON",
            DestinationNote = "Order with photos",
            CreatedByUserId = AdminUserId,
            CarrierId = CarrierSelfId,
            DeclaredQty = 10,
            ActualQty = 10,
            SpendCents = 100,
            CreatedAt = now.AddDays(-20),
            CompletedAt = now.AddDays(-5),
            NextAction = new OrderNextAction
            {
                NextActionKind = NextActionKind.Paid,
                NextActionLabel = "Paid",
                NextActionAmountCents = 100,
                NextActionDocumentNumber = "D01812"
            },
            Cabinet = new OrderCabinetDetail
            {
                CustomerName = "R-way Transport",
                PrimaryReference = "REF-1007",
                ServicesCsv = "Unloading,Loading"
            },
            Dock = new OrderDockAssignment
            {
                DockCode = "D1",
                DockBay = "Bay A",
                DockStatusLabel = "Completed",
                AssignedToUserId = AdminUserId
            },
            QuantityLines =
            {
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.XL, Count = 10 }
            },
            SubOrders =
            {
                new SubOrder { Id = Guid.NewGuid(), Number = "FR001672-1", Reference = "REF-1007", PalletCount = 10, SortOrder = 1 }
            }
        };

        var draft = new Order
        {
            Id = DraftOrderId,
            Number = "DRAFT-000101",
            Type = OrderType.CrossDock,
            Status = OrderStatus.Draft,
            HubId = HubMarkhamId,
            ScheduledAt = now.AddDays(3),
            DestinationCity = "Toronto",
            DestinationRegion = "ON",
            CreatedByUserId = AdminUserId,
            CreatedAt = now.AddHours(-2),
            Cabinet = new OrderCabinetDetail { CustomerName = "Draft Customer", PrimaryReference = "DRAFT-REF" },
            Dock = new OrderDockAssignment { AssignedToUserId = DispatcherUserId }
        };

        var closed = new Order
        {
            Id = ClosedOrderId,
            Number = "FR001700",
            Type = OrderType.CrossDock,
            Status = OrderStatus.Closed,
            HubId = HubTorontoId,
            ScheduledAt = now.AddDays(-30),
            DestinationCity = "Ottawa",
            DestinationRegion = "ON",
            CreatedByUserId = DispatcherUserId,
            CarrierId = CarrierSchneiderId,
            DeclaredQty = 5,
            ActualQty = 5,
            CreatedAt = now.AddDays(-35),
            CompletedAt = now.AddDays(-28),
            NextAction = new OrderNextAction
            {
                NextActionKind = NextActionKind.Closed,
                NextActionLabel = "Closed"
            },
            Cabinet = new OrderCabinetDetail { CustomerName = "Closed Co.", PrimaryReference = "REF-CLOSED" },
            Dock = new OrderDockAssignment
            {
                DockCode = "T1",
                DockStatusLabel = "Completed",
                AssignedToUserId = DriverUserId
            },
            QuantityLines =
            {
                new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 5 }
            }
        };

        var more = new List<Order>();
        for (var i = 0; i < 4; i++)
        {
            more.Add(new Order
            {
                Id = Guid.Parse($"c0000000-0000-0000-0000-00000001000{i}"),
                Number = $"FR00169{i}",
                Type = OrderType.CrossDock,
                Status = OrderStatus.InProgress,
                HubId = HubMarkhamId,
                ScheduledAt = now.AddDays(i),
                DestinationCity = "Toronto",
                DestinationRegion = "ON",
                CreatedByUserId = AdminUserId,
                CarrierId = CarrierTForceId,
                DeclaredQty = 5 + i,
                ActualQty = 5 + i,
                CreatedAt = now.AddDays(-i),
                NextAction = new OrderNextAction
                {
                    NextActionLabel = "Loading",
                    NextActionKind = NextActionKind.Loading
                },
                Cabinet = new OrderCabinetDetail
                {
                    CustomerName = "Ontario Crossdock Ltd.",
                    PrimaryReference = $"REF-11{i:00}"
                },
                Dock = new OrderDockAssignment
                {
                    DockCode = i % 2 == 0 ? "D1" : "D2",
                    DockBay = i % 2 == 0 ? "Bay A" : "Bay B",
                    AssignedToUserId = i % 2 == 0 ? DriverUserId : DispatcherUserId,
                    DockStatusLabel = "Trailer docked · loading"
                },
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

        for (var w = 0; w < 8; w++)
        {
            var completedAt = now.AddDays(-(7 - w) * 7);
            more.Add(new Order
            {
                Id = Guid.NewGuid(),
                Number = $"FR00{2100 + w}",
                Type = OrderType.CrossDock,
                Status = OrderStatus.Completed,
                HubId = HubMarkhamId,
                ScheduledAt = completedAt.AddDays(-1),
                DestinationCity = "Toronto",
                DestinationRegion = "ON",
                CreatedByUserId = AdminUserId,
                CarrierId = CarrierSelfId,
                DeclaredQty = 4,
                ActualQty = 4,
                CreatedAt = completedAt.AddDays(-3),
                CompletedAt = completedAt,
                SpendCents = w == 3 ? 100 : 0,
                NextAction = new OrderNextAction
                {
                    NextActionKind = NextActionKind.Paid,
                    NextActionLabel = "Paid",
                    NextActionAmountCents = w == 3 ? 100 : 0
                },
                Cabinet = new OrderCabinetDetail { CustomerName = "Maple Leaf Distribution" },
                Dock = new OrderDockAssignment { AssignedToUserId = AdminUserId, DockStatusLabel = "Completed" },
                QuantityLines =
                {
                    new OrderQuantityLine { Id = Guid.NewGuid(), Unit = PalletUnit.Standard, Count = 4 }
                }
            });
        }

        db.Orders.AddRange(order676, order681, order674, order672, draft, closed);
        db.Orders.AddRange(more);

        // Operations on hero order
        db.OrderOperations.AddRange(
            new OrderOperation
            {
                Id = Op676LoadingId,
                OrderId = Order676Id,
                Type = OrderOperationType.Loading,
                Trailer = "TRL-8801",
                Quantity = 18,
                Unit = PalletUnit.Standard,
                UnitLabel = "Pallets",
                AppliedAt = now.AddHours(-4)
            },
            new OrderOperation
            {
                Id = Op676UnloadingId,
                OrderId = Order676Id,
                Type = OrderOperationType.Unloading,
                Trailer = "TRL-8801",
                Quantity = 18,
                Unit = PalletUnit.Standard,
                UnitLabel = "Pallets",
                AppliedAt = now.AddHours(-8)
            });

        db.OrderOperationComments.Add(new OrderOperationComment
        {
            Id = Guid.NewGuid(),
            OperationId = Op676LoadingId,
            Text = "Started loading bay A.",
            AuthorName = "Admin User",
            CreatedAt = now.AddHours(-3)
        });

        Guid opPhotoId = Guid.Parse("a3000000-0000-0000-0000-000000000001");
        string opKey = PhotoStorageKeys.ForOperation(Op676LoadingId, opPhotoId, "image/png");
        await photos.SaveAsync(opKey, TinyPng, CancellationToken.None);
        db.OrderOperationPhotos.Add(new OrderOperationPhoto
        {
            Id = opPhotoId,
            OperationId = Op676LoadingId,
            FileName = "loading.png",
            ContentType = "image/png",
            StorageKey = opKey
        });

        // Supplies (Cross-Dock payable sample on 681)
        db.OrderSupplies.AddRange(
            Supply(Order681Id, "WRAP-001", "Shrink wrap 120g", "Packaging", 2, 120),
            Supply(Order681Id, "PAL-STD", "Standard pallet", "Pallets", 5, 1500),
            Supply(Order676Id, "STRAP-12", "Straps 12'", "Securing", 4, 80));

        Guid whPhotoId = Guid.Parse("a3000000-0000-0000-0000-000000000002");
        string whKey = PhotoStorageKeys.ForWarehouse(Order676Id, whPhotoId, "image/png");
        await photos.SaveAsync(whKey, TinyPng, CancellationToken.None);
        db.OrderWarehousePhotos.Add(new OrderWarehousePhoto
        {
            Id = whPhotoId,
            OrderId = Order676Id,
            FileName = "dock.png",
            ContentType = "image/png",
            StorageKey = whKey
        });

        db.OrderComments.AddRange(
            new OrderComment
            {
                Id = Guid.NewGuid(),
                OrderId = Order676Id,
                Text = "Customer confirmed pallet count.",
                AuthorName = "Dispatcher User",
                CreatedAt = now.AddDays(-1)
            },
            new OrderComment
            {
                Id = Guid.NewGuid(),
                OrderId = Order674Id,
                Text = "Waiting on missing photo.",
                AuthorName = "Dispatcher User",
                CreatedAt = now.AddHours(-12)
            });

        db.OrderTimelineEntries.AddRange(
            new OrderTimelineEntry
            {
                Id = Guid.NewGuid(),
                OrderId = Order676Id,
                Kind = "Status",
                Text = "Moved to In Progress",
                PreviousStatus = OrderStatus.New,
                NewStatus = OrderStatus.InProgress,
                AuthorName = "Admin User",
                CreatedAt = now.AddDays(-9)
            },
            new OrderTimelineEntry
            {
                Id = Guid.NewGuid(),
                OrderId = Order676Id,
                Kind = "Manual",
                Text = "Dock D1 assigned.",
                AuthorName = "Dispatcher User",
                CreatedAt = now.AddDays(-1)
            });

        await db.SaveChangesAsync();
    }

    private static OrderSupply Supply(
        Guid orderId, string sku, string name, string category, int qty, long unitPrice) =>
        new()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Sku = sku,
            Name = name,
            Category = category,
            Quantity = qty,
            UnitPriceCents = unitPrice,
            LineTotalCents = qty * unitPrice
        };
}
