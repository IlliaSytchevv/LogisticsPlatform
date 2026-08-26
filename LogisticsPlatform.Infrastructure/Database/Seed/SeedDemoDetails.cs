using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Services;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsPlatform.Infrastructure.Database.Seed;

public static class SeedDemoDetails
{
    private static readonly Guid User1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid User2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid User3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid HubMarkhamId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    private static readonly Guid HubTorontoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

    private static readonly Guid CarrierSchneiderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");

    private static readonly Guid Order676Id = Guid.Parse("c0000000-0000-0000-0000-000000001676");
    private static readonly Guid Order674Id = Guid.Parse("c0000000-0000-0000-0000-000000001674");
    private static readonly Guid Order672Id = Guid.Parse("c0000000-0000-0000-0000-000000001672");
    private static readonly Guid DraftOrderId = Guid.Parse("c0000000-0000-0000-0000-000000000101");
    private static readonly Guid ClosedOrderId = Guid.Parse("c0000000-0000-0000-0000-000000000102");

    private static readonly Guid Op676LoadingId = Guid.Parse("a2000000-0000-0000-0000-000000000001");
    private static readonly Guid Op676UnloadingId = Guid.Parse("a2000000-0000-0000-0000-000000000002");
    private static readonly Guid Op676RestackId = Guid.Parse("a2000000-0000-0000-0000-000000000003");

    private static readonly byte[] TinyPng =
        Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

    public static async Task<SeedDemoDetailsResult> InitializeAsync(IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IPhotoBlobStore photoBlobStore = scope.ServiceProvider.GetRequiredService<IPhotoBlobStore>();

        var now = DateTimeOffset.UtcNow;

        int hubDocksAdded = await SeedHubDocksAsync(dbContext);
        int ordersEnriched = await EnrichHeroOrdersAsync(dbContext, now);
        int ordersAdded = await SeedExtraOrdersAsync(dbContext, now);
        int operationsAdded = await SeedOperationsAsync(dbContext, now);
        int operationCommentsAdded = await SeedOperationCommentsAsync(dbContext, now);
        int operationPhotosAdded = await SeedOperationPhotosAsync(dbContext, photoBlobStore);
        int suppliesAdded = await SeedSuppliesAsync(dbContext);
        int warehousePhotosAdded = await SeedWarehousePhotosAsync(dbContext, photoBlobStore);
        int commentsAdded = await SeedOrderCommentsAsync(dbContext, now);
        int timelineEntriesAdded = await SeedTimelineEntriesAsync(dbContext, now);

        await dbContext.SaveChangesAsync();

        return new SeedDemoDetailsResult(
            hubDocksAdded,
            ordersEnriched,
            ordersAdded,
            operationsAdded,
            operationCommentsAdded,
            operationPhotosAdded,
            suppliesAdded,
            warehousePhotosAdded,
            commentsAdded,
            timelineEntriesAdded);
    }

    private static async Task<int> SeedHubDocksAsync(AppDbContext dbContext)
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

        return added;
    }

    private static async Task<int> EnrichHeroOrdersAsync(AppDbContext dbContext, DateTimeOffset now)
    {
        int enriched = 0;

        Order? order676 = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == Order676Id);
        if (order676 is not null)
        {
            order676.Cabinet.CustomerName = "Acme Logistics Inc.";
            order676.Cabinet.PrimaryReference = "REF-ACME-676";
            order676.Cabinet.Phone = "+1 (416) 555-0100";
            order676.Cabinet.TrailerType = "53' Dry Van";
            order676.Cabinet.TruckNumber = "TRK-9021";
            order676.Cabinet.TrailerNumber = "TRL-4410";
            order676.Cabinet.ServicesCsv = "Cross-dock,Restack,Pallet wrap";
            order676.Cabinet.QuantityUnitLabel = "pallets";
            order676.Cabinet.StockStatusLabel = "In stock";
            order676.Cabinet.LoadingStatusLabel = "Loading in progress";
            order676.Dock.DockCode = "D1";
            order676.Dock.DockBay = "Bay A";
            order676.Dock.DockAssignedAt = now.AddHours(-1);
            order676.Dock.DockStatusLabel = "Assigned";
            order676.Dock.AssignedToUserId = User3Id;
            order676.Dock.WarehouseNote = "Priority lane — call dispatcher 15 min before arrival.";
            order676.DeclaredQty = 18;
            order676.ActualQty = 18;
            enriched++;
        }

        Order? order674 = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == Order674Id);
        if (order674 is not null)
        {
            order674.Cabinet.CustomerName = "Northern Freight Co.";
            order674.Cabinet.PrimaryReference = "REF-NFC-674";
            order674.Cabinet.Phone = "+1 (403) 555-0199";
            order674.Cabinet.TrailerType = "48' Reefer";
            order674.Cabinet.TruckNumber = "TRK-7733";
            order674.Cabinet.TrailerNumber = "TRL-2201";
            order674.Cabinet.ServicesCsv = "Consolidation,Photo check";
            order674.Cabinet.QuantityUnitLabel = "pallets";
            order674.Cabinet.StockStatusLabel = "Partial";
            order674.Cabinet.LoadingStatusLabel = "Awaiting photos";
            order674.Dock.DockCode = "D2";
            order674.Dock.DockBay = "Bay B";
            order674.Dock.DockStatusLabel = "Waiting";
            order674.Dock.WarehouseNote = "Missing sub-order photo — upload before dispatch.";
            enriched++;
        }

        Order? order672 = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == Order672Id);
        if (order672 is not null)
        {
            order672.Cabinet.CustomerName = "Brampton Retail Hub";
            order672.Cabinet.PrimaryReference = "REF-BRH-672";
            order672.Cabinet.Phone = "+1 (905) 555-0144";
            order672.Cabinet.TruckNumber = "TRK-1102";
            order672.Cabinet.TrailerNumber = "TRL-8890";
            order672.Cabinet.QuantityUnitLabel = "pallets";
            order672.Dock.DockCode = "D1";
            order672.Dock.DockBay = "Bay A";
            order672.Dock.DockAssignedAt = now.AddDays(-6);
            order672.Dock.DockStatusLabel = "Completed";
            enriched++;
        }

        return enriched;
    }

    private static async Task<int> SeedExtraOrdersAsync(AppDbContext dbContext, DateTimeOffset now)
    {
        int added = 0;

        if (!await dbContext.Orders.AnyAsync(o => o.Id == DraftOrderId))
        {
            dbContext.Orders.Add(new Order
            {
                Id = DraftOrderId,
                Number = "DRAFT-001",
                Type = OrderType.CrossDock,
                Status = OrderStatus.Draft,
                HubId = HubMarkhamId,
                ScheduledAt = now.AddDays(3),
                DestinationCity = "Montreal",
                DestinationRegion = "QC",
                CreatedByUserId = User1Id,
                CreatedAt = now.AddHours(-2),
                Cabinet = new OrderCabinetDetail { PrimaryReference = "DRAFT-REF-001" },
                NextAction = new OrderNextAction { NextActionLabel = "Continue editing" },
                TimelineEntries =
                {
                    new OrderTimelineEntry
                    {
                        Id = Guid.Parse("a6000000-0000-0000-0000-000000000001"),
                        Kind = "Status",
                        Text = string.Empty,
                        PreviousStatus = null,
                        NewStatus = OrderStatus.Draft,
                        CreatedAt = now.AddHours(-2)
                    }
                }
            });
            added++;
        }

        if (!await dbContext.Orders.AnyAsync(o => o.Id == ClosedOrderId))
        {
            dbContext.Orders.Add(new Order
            {
                Id = ClosedOrderId,
                Number = "FR009999",
                Type = OrderType.Consolidation,
                Status = OrderStatus.Closed,
                HubId = HubTorontoId,
                ScheduledAt = now.AddDays(-30),
                DestinationCity = "Windsor",
                DestinationRegion = "ON",
                CreatedByUserId = User2Id,
                CarrierId = CarrierSchneiderId,
                CreatedAt = now.AddDays(-35),
                CompletedAt = now.AddDays(-28),
                Cabinet = new OrderCabinetDetail
                {
                    CustomerName = "Closed Order Customer",
                    PrimaryReference = "REF-CLOSED-9999"
                },
                NextAction = new OrderNextAction
                {
                    NextActionKind = NextActionKind.Paid,
                    NextActionLabel = "Paid",
                    NextActionAmountCents = 250,
                    NextActionDocumentNumber = "D01999"
                },
                SpendCents = 250,
                QuantityLines =
                {
                    new OrderQuantityLine
                    {
                        Id = Guid.Parse("a7000000-0000-0000-0000-000000000001"),
                        Unit = PalletUnit.Standard,
                        Count = 6
                    }
                },
                SubOrders =
                {
                    new SubOrder
                    {
                        Id = Guid.Parse("a7000000-0000-0000-0000-000000000002"),
                        Number = "FR009999-1",
                        Reference = "REF-9999",
                        PalletCount = 6,
                        SortOrder = 1
                    }
                }
            });
            added++;
        }

        return added;
    }

    private static async Task<int> SeedOperationsAsync(AppDbContext dbContext, DateTimeOffset now)
    {
        if (!await dbContext.Orders.AnyAsync(o => o.Id == Order676Id))
            return 0;

        OrderOperation[] operations =
        [
            new()
            {
                Id = Op676LoadingId,
                OrderId = Order676Id,
                Type = OrderOperationType.Loading,
                Trailer = "TRL-4410",
                Quantity = 12,
                Unit = PalletUnit.Standard,
                UnitLabel = "Std pallets",
                AppliedAt = now.AddHours(-3)
            },
            new()
            {
                Id = Op676UnloadingId,
                OrderId = Order676Id,
                Type = OrderOperationType.Unloading,
                Trailer = "TRL-4410",
                Quantity = 6,
                Unit = PalletUnit.Standard,
                UnitLabel = "Std pallets",
                AppliedAt = now.AddHours(-2)
            },
            new()
            {
                Id = Op676RestackId,
                OrderId = Order676Id,
                Type = OrderOperationType.Restack,
                Trailer = null,
                Quantity = 3,
                Unit = PalletUnit.XL,
                UnitLabel = "XL pallets",
                AppliedAt = now.AddHours(-1)
            }
        ];

        int added = 0;
        foreach (OrderOperation operation in operations)
        {
            if (await dbContext.OrderOperations.AnyAsync(x => x.Id == operation.Id))
                continue;

            dbContext.OrderOperations.Add(operation);
            added++;
        }

        return added;
    }

    private static async Task<int> SeedOperationCommentsAsync(AppDbContext dbContext, DateTimeOffset now)
    {
        OrderOperationComment[] comments =
        [
            new()
            {
                Id = Guid.Parse("a2100000-0000-0000-0000-000000000001"),
                OperationId = Op676LoadingId,
                Text = "Started loading — team on bay A.",
                AuthorName = "User 3",
                CreatedAt = now.AddHours(-3)
            },
            new()
            {
                Id = Guid.Parse("a2100000-0000-0000-0000-000000000002"),
                OperationId = Op676LoadingId,
                Text = "Seal verified, ready for departure.",
                AuthorName = "User 1",
                CreatedAt = now.AddHours(-2).AddMinutes(45)
            }
        ];

        int added = 0;
        foreach (OrderOperationComment comment in comments)
        {
            if (await dbContext.OrderOperationComments.AnyAsync(x => x.Id == comment.Id))
                continue;

            if (!await dbContext.OrderOperations.AnyAsync(x => x.Id == comment.OperationId))
                continue;

            dbContext.OrderOperationComments.Add(comment);
            added++;
        }

        return added;
    }

    private static async Task<int> SeedOperationPhotosAsync(
        AppDbContext dbContext,
        IPhotoBlobStore photoBlobStore)
    {
        var photoId = Guid.Parse("a2200000-0000-0000-0000-000000000001");
        if (await dbContext.OrderOperationPhotos.AnyAsync(x => x.Id == photoId))
            return 0;

        if (!await dbContext.OrderOperations.AnyAsync(x => x.Id == Op676LoadingId))
            return 0;

        const string contentType = "image/png";
        string storageKey = PhotoStorageKeys.ForOperation(Op676LoadingId, photoId, contentType);
        await photoBlobStore.SaveAsync(storageKey, TinyPng, CancellationToken.None);

        dbContext.OrderOperationPhotos.Add(new OrderOperationPhoto
        {
            Id = photoId,
            OperationId = Op676LoadingId,
            FileName = "loading-bay-a.png",
            ContentType = contentType,
            StorageKey = storageKey
        });

        return 1;
    }

    private static async Task<int> SeedSuppliesAsync(AppDbContext dbContext)
    {
        if (!await dbContext.Orders.AnyAsync(o => o.Id == Order676Id))
            return 0;

        OrderSupply[] supplies =
        [
            new()
            {
                Id = Guid.Parse("a3000000-0000-0000-0000-000000000001"),
                OrderId = Order676Id,
                Sku = "WRAP-001",
                Name = "Stretch wrap roll",
                Category = "Packaging",
                Quantity = 4,
                UnitPriceCents = 1250,
                LineTotalCents = 5000
            },
            new()
            {
                Id = Guid.Parse("a3000000-0000-0000-0000-000000000002"),
                OrderId = Order676Id,
                Sku = "LBL-050",
                Name = "Shipping labels pack",
                Category = "Labels",
                Quantity = 2,
                UnitPriceCents = 899,
                LineTotalCents = 1798
            },
            new()
            {
                Id = Guid.Parse("a3000000-0000-0000-0000-000000000003"),
                OrderId = Order676Id,
                Sku = "PAL-STD",
                Name = "Standard pallet",
                Category = "Pallets",
                Quantity = 10,
                UnitPriceCents = 1500,
                LineTotalCents = 15000
            }
        ];

        int added = 0;
        foreach (OrderSupply supply in supplies)
        {
            if (await dbContext.OrderSupplies.AnyAsync(x => x.Id == supply.Id))
                continue;

            dbContext.OrderSupplies.Add(supply);
            added++;
        }

        return added;
    }

    private static async Task<int> SeedWarehousePhotosAsync(
        AppDbContext dbContext,
        IPhotoBlobStore photoBlobStore)
    {
        (Guid Id, Guid OrderId, string FileName)[] photos =
        [
            (Guid.Parse("a4000000-0000-0000-0000-000000000001"), Order672Id, "warehouse-front.png"),
            (Guid.Parse("a4000000-0000-0000-0000-000000000002"), Order672Id, "warehouse-seal.png"),
            (Guid.Parse("a4000000-0000-0000-0000-000000000003"), Order676Id, "dock-overview.png")
        ];

        int added = 0;
        foreach ((Guid id, Guid orderId, string fileName) in photos)
        {
            if (await dbContext.OrderWarehousePhotos.AnyAsync(x => x.Id == id))
                continue;

            if (!await dbContext.Orders.AnyAsync(o => o.Id == orderId))
                continue;

            const string contentType = "image/png";
            string storageKey = PhotoStorageKeys.ForWarehouse(orderId, id, contentType);
            await photoBlobStore.SaveAsync(storageKey, TinyPng, CancellationToken.None);

            dbContext.OrderWarehousePhotos.Add(new OrderWarehousePhoto
            {
                Id = id,
                OrderId = orderId,
                FileName = fileName,
                ContentType = contentType,
                StorageKey = storageKey
            });
            added++;
        }

        return added;
    }

    private static async Task<int> SeedOrderCommentsAsync(AppDbContext dbContext, DateTimeOffset now)
    {
        OrderComment[] comments =
        [
            new()
            {
                Id = Guid.Parse("a5000000-0000-0000-0000-000000000001"),
                OrderId = Order676Id,
                Text = "Customer confirmed pickup window 09:00–11:00.",
                AuthorName = "User 2",
                CreatedAt = now.AddDays(-1)
            },
            new()
            {
                Id = Guid.Parse("a5000000-0000-0000-0000-000000000002"),
                OrderId = Order676Id,
                Text = "Dock assigned — notify warehouse lead.",
                AuthorName = "User 3",
                CreatedAt = now.AddHours(-5)
            },
            new()
            {
                Id = Guid.Parse("a5000000-0000-0000-0000-000000000003"),
                OrderId = Order674Id,
                Text = "Alert: missing photo on sub-order REF-1006.",
                AuthorName = "User 1",
                CreatedAt = now.AddDays(-2)
            }
        ];

        int added = 0;
        foreach (OrderComment comment in comments)
        {
            if (await dbContext.OrderComments.AnyAsync(x => x.Id == comment.Id))
                continue;

            if (!await dbContext.Orders.AnyAsync(o => o.Id == comment.OrderId))
                continue;

            dbContext.OrderComments.Add(comment);
            added++;
        }

        return added;
    }

    private static async Task<int> SeedTimelineEntriesAsync(AppDbContext dbContext, DateTimeOffset now)
    {
        OrderTimelineEntry[] entries =
        [
            new()
            {
                Id = Guid.Parse("a6000000-0000-0000-0000-000000000010"),
                OrderId = Order676Id,
                Kind = "Status",
                Text = string.Empty,
                PreviousStatus = OrderStatus.New,
                NewStatus = OrderStatus.InProgress,
                AuthorName = "System",
                CreatedAt = now.AddDays(-9)
            },
            new()
            {
                Id = Guid.Parse("a6000000-0000-0000-0000-000000000011"),
                OrderId = Order676Id,
                Kind = "Manual",
                Text = "Dock D1 / Bay A assigned to User 3.",
                AuthorName = "User 2",
                CreatedAt = now.AddHours(-4)
            },
            new()
            {
                Id = Guid.Parse("a6000000-0000-0000-0000-000000000012"),
                OrderId = Order676Id,
                Kind = "Manual",
                Text = "Loading started on bay A.",
                AuthorName = "User 3",
                CreatedAt = now.AddHours(-3)
            },
            new()
            {
                Id = Guid.Parse("a6000000-0000-0000-0000-000000000013"),
                OrderId = Order674Id,
                Kind = "Status",
                Text = string.Empty,
                PreviousStatus = OrderStatus.InProgress,
                NewStatus = OrderStatus.Alert,
                AuthorName = "System",
                CreatedAt = now.AddDays(-7)
            }
        ];

        int added = 0;
        foreach (OrderTimelineEntry entry in entries)
        {
            if (await dbContext.OrderTimelineEntries.AnyAsync(x => x.Id == entry.Id))
                continue;

            if (!await dbContext.Orders.AnyAsync(o => o.Id == entry.OrderId))
                continue;

            dbContext.OrderTimelineEntries.Add(entry);
            added++;
        }

        return added;
    }
}

public sealed record SeedDemoDetailsResult(
    int HubDocksAdded,
    int OrdersEnriched,
    int OrdersAdded,
    int OperationsAdded,
    int OperationCommentsAdded,
    int OperationPhotosAdded,
    int SuppliesAdded,
    int WarehousePhotosAdded,
    int CommentsAdded,
    int TimelineEntriesAdded);
