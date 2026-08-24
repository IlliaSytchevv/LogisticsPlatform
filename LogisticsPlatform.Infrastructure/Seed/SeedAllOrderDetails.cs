using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsPlatform.Infrastructure.Seed;

public static class SeedAllOrderDetails
{
    private static readonly Guid User1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid User2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid User3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid HubMarkhamId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    private static readonly Guid HubTorontoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

    private static readonly Guid[] AssigneeIds = [User1Id, User2Id, User3Id];

    private static readonly string[] Customers =
    [
        "Acme Logistics Inc.",
        "Northern Freight Co.",
        "Brampton Retail Hub",
        "R-way Transport",
        "Great Lakes Shipping",
        "Ontario Crossdock Ltd.",
        "Maple Leaf Distribution",
        "Toronto Hub Partners"
    ];

    private static readonly string[] ServicesSets =
    [
        "Transload,Restack",
        "Cross-dock,Pallet wrap",
        "Consolidation,Photo check",
        "Unloading,Loading",
        "Restock & Rework,Transload"
    ];

    private static readonly string[] TrailerTypes =
    [
        "Van · 53ft",
        "53' Dry Van",
        "48' Reefer",
        "Dry Van · 48ft"
    ];

    private static readonly byte[] TinyPng =
        Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

    public static async Task<SeedAllOrderDetailsResult> InitializeAsync(IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTimeOffset.UtcNow;

        int hubDocksAdded = await EnsureHubDocksAsync(dbContext);

        List<HubDock> hubDocks = await dbContext.HubDocks
            .AsNoTracking()
            .OrderBy(d => d.SortOrder)
            .ToListAsync();

        List<Order> orders = await dbContext.Orders
            .Include(o => o.QuantityLines)
            .Include(o => o.SubOrders)
            .ToListAsync();

        HashSet<Guid> ordersWithOps = (await dbContext.OrderOperations
                .Where(x => !x.IsDeleted)
                .Select(x => x.OrderId)
                .Distinct()
                .ToListAsync())
            .ToHashSet();

        HashSet<Guid> ordersWithSupplies = (await dbContext.OrderSupplies
                .Where(x => !x.IsDeleted)
                .Select(x => x.OrderId)
                .Distinct()
                .ToListAsync())
            .ToHashSet();

        HashSet<Guid> ordersWithComments = (await dbContext.OrderComments
                .Select(x => x.OrderId)
                .Distinct()
                .ToListAsync())
            .ToHashSet();

        HashSet<Guid> ordersWithTimeline = (await dbContext.OrderTimelineEntries
                .Select(x => x.OrderId)
                .Distinct()
                .ToListAsync())
            .ToHashSet();

        HashSet<Guid> ordersWithWarehousePhotos = (await dbContext.OrderWarehousePhotos
                .Select(x => x.OrderId)
                .Distinct()
                .ToListAsync())
            .ToHashSet();

        int ordersEnriched = 0;
        int operationsAdded = 0;
        int suppliesAdded = 0;
        int commentsAdded = 0;
        int timelineEntriesAdded = 0;
        int warehousePhotosAdded = 0;

        for (var i = 0; i < orders.Count; i++)
        {
            Order order = orders[i];
            if (EnrichCabinetAndDock(order, hubDocks, i))
                ordersEnriched++;

            if (!ordersWithOps.Contains(order.Id))
            {
                operationsAdded += AddOperations(dbContext, order, i);
                ordersWithOps.Add(order.Id);
            }

            if (!ordersWithSupplies.Contains(order.Id))
            {
                suppliesAdded += AddSupplies(dbContext, order, i);
                ordersWithSupplies.Add(order.Id);
            }

            if (!ordersWithComments.Contains(order.Id))
            {
                commentsAdded += AddComment(dbContext, order, now, i);
                ordersWithComments.Add(order.Id);
            }

            if (!ordersWithTimeline.Contains(order.Id))
            {
                timelineEntriesAdded += AddTimeline(dbContext, order, now, i);
                ordersWithTimeline.Add(order.Id);
            }

            if (!ordersWithWarehousePhotos.Contains(order.Id))
            {
                warehousePhotosAdded += AddWarehousePhoto(dbContext, order);
                ordersWithWarehousePhotos.Add(order.Id);
            }
        }

        await dbContext.SaveChangesAsync();

        return new SeedAllOrderDetailsResult(
            hubDocksAdded,
            orders.Count,
            ordersEnriched,
            operationsAdded,
            suppliesAdded,
            commentsAdded,
            timelineEntriesAdded,
            warehousePhotosAdded);
    }

    private static bool EnrichCabinetAndDock(
        Order order,
        IReadOnlyList<HubDock> hubDocks,
        int index)
    {
        bool changed = false;

        int qtyFromLines = order.QuantityLines.Sum(l => l.Count);
        if (qtyFromLines <= 0)
            qtyFromLines = 8 + index % 12;

        if (order.DeclaredQty is null)
        {
            order.DeclaredQty = qtyFromLines;
            changed = true;
        }

        if (order.ActualQty is null)
        {
            order.ActualQty = index % 4 == 0
                ? order.DeclaredQty + (index % 2 == 0 ? 2 : -1)
                : order.DeclaredQty;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.CustomerName))
        {
            order.Cabinet.CustomerName = Customers[index % Customers.Length];
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.PrimaryReference))
        {
            order.Cabinet.PrimaryReference = order.SubOrders
                    .OrderBy(s => s.SortOrder)
                    .Select(s => s.Reference)
                    .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r))
                ?? $"REF-{order.Number}";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.Phone))
        {
            order.Cabinet.Phone = $"+1 (416) 555-{(1000 + index % 9000):D4}";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.TrailerType))
        {
            order.Cabinet.TrailerType = TrailerTypes[index % TrailerTypes.Length];
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.TruckNumber))
        {
            order.Cabinet.TruckNumber = $"TRK-{(4000 + index):D4}";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.TrailerNumber))
        {
            order.Cabinet.TrailerNumber = $"TRL-{(8000 + index):D4}";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.ServicesCsv))
        {
            order.Cabinet.ServicesCsv = ServicesSets[index % ServicesSets.Length];
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.QuantityUnitLabel))
        {
            order.Cabinet.QuantityUnitLabel = "Standard · 48×40";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.StockStatusLabel))
        {
            order.Cabinet.StockStatusLabel = order.Status switch
            {
                OrderStatus.Draft => "Not received",
                OrderStatus.Completed or OrderStatus.Closed => "Cleared",
                OrderStatus.Alert => "Partial",
                _ => "On Stock"
            };
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Cabinet.LoadingStatusLabel))
        {
            order.Cabinet.LoadingStatusLabel = order.Status switch
            {
                OrderStatus.InProgress => "Loading in progress",
                OrderStatus.New => "Awaiting truck",
                OrderStatus.Alert => "On hold",
                OrderStatus.Completed or OrderStatus.Closed => "Loaded",
                OrderStatus.Draft => "Not started",
                _ => "Queued"
            };
            changed = true;
        }

        List<HubDock> docksForHub = hubDocks.Where(d => d.HubId == order.HubId).ToList();
        if (docksForHub.Count == 0)
            docksForHub = hubDocks.ToList();

        HubDock? dock = docksForHub.Count > 0
            ? docksForHub[index % docksForHub.Count]
            : null;

        if (string.IsNullOrWhiteSpace(order.Dock.DockCode) && dock is not null)
        {
            order.Dock.DockCode = dock.Code;
            order.Dock.DockBay = dock.BayLabel;
            order.Dock.DockAssignedAt ??= order.ScheduledAt.AddHours(-2);
            order.Dock.DockStatusLabel ??= order.Status switch
            {
                OrderStatus.Completed or OrderStatus.Closed => "Completed",
                OrderStatus.InProgress => "Trailer docked · loading",
                OrderStatus.Alert => "Waiting",
                _ => "Assigned"
            };
            order.Dock.AssignedToUserId ??= AssigneeIds[index % AssigneeIds.Length];
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(order.Dock.WarehouseNote))
        {
            order.Dock.WarehouseNote =
                $"Seeded note for {order.Number}: counted {order.ActualQty} pallets on arrival (BOL {order.DeclaredQty}).";
            changed = true;
        }

        return changed;
    }

    private static int AddOperations(AppDbContext dbContext, Order order, int index)
    {
        int qty = order.ActualQty ?? order.DeclaredQty ?? 10;
        string trailer = order.Cabinet.TrailerNumber ?? $"TRL-{(8000 + index):D4}";

        var ops = new List<OrderOperation>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Type = OrderOperationType.Unloading,
                Trailer = trailer,
                Quantity = qty,
                Unit = PalletUnit.Standard,
                UnitLabel = "Standard (48×40)",
                AppliedAt = order.ScheduledAt.AddMinutes(30)
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Type = OrderOperationType.Loading,
                Trailer = trailer,
                Quantity = Math.Max(1, qty - (index % 4 == 0 ? 1 : 0)),
                Unit = PalletUnit.Standard,
                UnitLabel = "Standard (48×40)",
                AppliedAt = order.ScheduledAt.AddHours(2)
            }
        };

        if (index % 3 == 0)
        {
            ops.Add(new OrderOperation
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Type = OrderOperationType.Restack,
                Trailer = null,
                Quantity = Math.Max(1, qty / 4),
                Unit = PalletUnit.Standard,
                UnitLabel = "Standard (48×40)",
                AppliedAt = order.ScheduledAt.AddHours(1)
            });
        }

        if (index % 5 == 0)
        {
            ops.Add(new OrderOperation
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Type = OrderOperationType.Disposal,
                Trailer = null,
                Quantity = 1,
                Unit = PalletUnit.Standard,
                UnitLabel = "Standard (48×40)",
                AppliedAt = order.ScheduledAt.AddMinutes(50)
            });
        }

        dbContext.OrderOperations.AddRange(ops);
        return ops.Count;
    }

    private static int AddSupplies(AppDbContext dbContext, Order order, int index)
    {
        OrderSupply[] supplies =
        [
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Sku = "STRAP-12",
                Name = "Straps 12",
                Category = "Securement",
                Quantity = 2 + index % 5,
                UnitPriceCents = 100,
                LineTotalCents = (2 + index % 5) * 100
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Sku = "WRAP-120",
                Name = "Shrink wrap 120g",
                Category = "Wrap",
                Quantity = 1 + index % 3,
                UnitPriceCents = 250,
                LineTotalCents = (1 + index % 3) * 250
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Sku = "CORNER-50",
                Name = "Corners 50",
                Category = "Edge protect",
                Quantity = 4 + index % 8,
                UnitPriceCents = 75,
                LineTotalCents = (4 + index % 8) * 75
            }
        ];

        dbContext.OrderSupplies.AddRange(supplies);
        return supplies.Length;
    }

    private static int AddComment(AppDbContext dbContext, Order order, DateTimeOffset now, int index)
    {
        dbContext.OrderComments.Add(new OrderComment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Text = $"Seeded comment for {order.Number}: ready for warehouse processing.",
            AuthorName = index % 2 == 0 ? "User 2" : "User 3",
            CreatedAt = now.AddHours(-(1 + index % 24))
        });
        return 1;
    }

    private static int AddTimeline(AppDbContext dbContext, Order order, DateTimeOffset now, int index)
    {
        dbContext.OrderTimelineEntries.AddRange(
            new OrderTimelineEntry
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Kind = "Status",
                Text = $"Created → {FormatStatus(order.Status)}",
                AuthorName = "System",
                CreatedAt = order.CreatedAt
            },
            new OrderTimelineEntry
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Kind = "Manual",
                Text = $"Detail seed applied for {order.Number}.",
                AuthorName = "User 1",
                CreatedAt = now.AddMinutes(-(index % 60))
            });
        return 2;
    }

    private static int AddWarehousePhoto(AppDbContext dbContext, Order order)
    {
        dbContext.OrderWarehousePhotos.Add(new OrderWarehousePhoto
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            FileName = $"warehouse-{order.Number}.png",
            ContentType = "image/png",
            Content = TinyPng,
            SortOrder = 1
        });
        return 1;
    }

    private static string FormatStatus(OrderStatus status) => status switch
    {
        OrderStatus.InProgress => "IN PROGRESS",
        OrderStatus.New => "NEW",
        OrderStatus.Alert => "ALERT",
        OrderStatus.Completed => "COMPLETED",
        OrderStatus.Closed => "CLOSED",
        OrderStatus.Draft => "DRAFT",
        _ => status.ToString().ToUpperInvariant()
    };

    private static async Task<int> EnsureHubDocksAsync(AppDbContext dbContext)
    {
        HubDock[] docks =
        [
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                HubId = HubMarkhamId,
                Code = "D1",
                BayLabel = "Bay A",
                SortOrder = 1
            },
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                HubId = HubMarkhamId,
                Code = "D2",
                BayLabel = "Bay B",
                SortOrder = 2
            },
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000003"),
                HubId = HubMarkhamId,
                Code = "D3",
                BayLabel = "Bay C",
                SortOrder = 3
            },
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000011"),
                HubId = HubTorontoId,
                Code = "T1",
                BayLabel = "Door 1",
                SortOrder = 1
            },
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000012"),
                HubId = HubTorontoId,
                Code = "T2",
                BayLabel = "Door 2",
                SortOrder = 2
            }
        ];

        int added = 0;
        foreach (HubDock dock in docks)
        {
            if (await dbContext.HubDocks.AnyAsync(d => d.Id == dock.Id))
                continue;

            dbContext.HubDocks.Add(dock);
            added++;
        }

        if (added > 0)
            await dbContext.SaveChangesAsync();

        return added;
    }
}

public sealed record SeedAllOrderDetailsResult(
    int HubDocksAdded,
    int OrdersScanned,
    int OrdersEnriched,
    int OperationsAdded,
    int SuppliesAdded,
    int CommentsAdded,
    int TimelineEntriesAdded,
    int WarehousePhotosAdded);
