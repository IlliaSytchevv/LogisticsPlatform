using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories;

public sealed class OrdersRepository(AppDbContext dbContext) : IOrdersRepository
{
    public async Task<OrdersListData> GetOrdersAsync(
        OrdersListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Order> query = OrdersQueryFilter.Apply(dbContext.Orders.AsNoTracking(), filter);

        int totalCount = await query.CountAsync(cancellationToken);

        var pageRows = await query
            .OrderByDescending(o => o.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                o.Id,
                o.Number,
                o.Type,
                o.Status,
                o.HasAlert,
                o.AlertReason,
                CreatedByName = o.CreatedByUser.DisplayName,
                CreatedByInitials = o.CreatedByUser.Initials,
                CreatedByRole = o.CreatedByUser.Role,
                Hub = o.Hub.Name,
                o.ScheduledAt,
                o.DeclaredQty,
                o.ActualQty,
                CarrierName = o.Carrier != null ? o.Carrier.Name : null,
                o.DestinationCity,
                o.DestinationRegion,
                o.DestinationNote,
                o.NextActionLabel,
                o.NextActionKind,
                o.NextActionDueAt,
                o.NextActionAmountCents,
                o.NextActionDocumentNumber
            })
            .ToListAsync(cancellationToken);

        if (pageRows.Count == 0)
            return new OrdersListData(totalCount, []);

        Guid[] orderIds = pageRows.Select(o => o.Id).ToArray();

        var quantityLines = await dbContext.OrderQuantityLines
            .AsNoTracking()
            .Where(x => orderIds.Contains(x.OrderId))
            .OrderBy(x => x.Unit)
            .Select(x => new { x.OrderId, x.Unit, x.Count })
            .ToListAsync(cancellationToken);

        var subOrders = await dbContext.SubOrders
            .AsNoTracking()
            .Where(x => orderIds.Contains(x.OrderId))
            .OrderBy(x => x.SortOrder)
            .Select(x => new { x.OrderId, x.Number, x.Reference, x.PalletCount, x.HasMissingPhoto })
            .ToListAsync(cancellationToken);

        var quantitiesByOrder = quantityLines
            .GroupBy(x => x.OrderId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<OrderListQuantityLineData>)g
                    .Select(x => new OrderListQuantityLineData(x.Unit, x.Count))
                    .ToList());

        var subOrdersByOrder = subOrders
            .GroupBy(x => x.OrderId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<OrderListSubOrderData>)g
                    .Select(x => new OrderListSubOrderData(
                        x.Number,
                        x.Reference,
                        x.PalletCount,
                        x.HasMissingPhoto))
                    .ToList());

        IReadOnlyList<OrderListItemData> items = pageRows
            .Select(o => new OrderListItemData(
                o.Id,
                o.Number,
                o.Type,
                o.Status,
                o.HasAlert,
                o.AlertReason,
                o.CreatedByName,
                o.CreatedByInitials,
                o.CreatedByRole,
                o.Hub,
                o.ScheduledAt,
                o.DeclaredQty,
                o.ActualQty,
                o.CarrierName,
                o.DestinationCity,
                o.DestinationRegion,
                o.DestinationNote,
                o.NextActionLabel,
                o.NextActionKind,
                o.NextActionDueAt,
                o.NextActionAmountCents,
                o.NextActionDocumentNumber,
                quantitiesByOrder.GetValueOrDefault(o.Id, []),
                subOrdersByOrder.GetValueOrDefault(o.Id, [])))
            .ToList();

        return new OrdersListData(totalCount, items);
    }

    public async Task<OrdersTabCountsData> GetTabCountsAsync(
        OrdersListFilter filter,
        CancellationToken cancellationToken)
    {
        OrdersListFilter baseFilter = filter with { Tab = null };
        IQueryable<Order> query = OrdersQueryFilter.Apply(dbContext.Orders.AsNoTracking(), baseFilter);

        var counts = await query
            .GroupBy(_ => 1)
            .Select(g => new
            {
                All = g.Count(),
                CrossDock = g.Count(o => o.Type == OrderType.CrossDock),
                Consolidation = g.Count(o => o.Type == OrderType.Consolidation),
                Alerts = g.Count(o => o.HasAlert || o.Status == OrderStatus.Alert),
                Drafts = g.Count(o => o.Status == OrderStatus.Draft)
            })
            .SingleOrDefaultAsync(cancellationToken);

        return new OrdersTabCountsData(
            counts?.All ?? 0,
            counts?.CrossDock ?? 0,
            counts?.Consolidation ?? 0,
            counts?.Alerts ?? 0,
            counts?.Drafts ?? 0);
    }

    public async Task<OrdersFilterOptionsData> GetFilterOptionsAsync(CancellationToken cancellationToken)
    {
        List<OrderHubOptionData> hubs = await dbContext.Hubs
            .AsNoTracking()
            .OrderBy(h => h.Name)
            .Select(h => new OrderHubOptionData(h.Id, h.Name))
            .ToListAsync(cancellationToken);

        return new OrdersFilterOptionsData(hubs);
    }

    public Task<bool> HubExistsAsync(Guid hubId, CancellationToken cancellationToken) =>
        dbContext.Hubs.AsNoTracking().AnyAsync(h => h.Id == hubId, cancellationToken);

    public Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken) =>
        dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId, cancellationToken);

    public async Task<OrderCreatedData> CreateDraftAsync(
        OrderType type,
        Guid hubId,
        Guid createdByUserId,
        DateTimeOffset scheduledAt,
        string destinationCity,
        string destinationRegion,
        string? primaryReference,
        CancellationToken cancellationToken)
    {
        int draftCount = await dbContext.Orders
            .IgnoreQueryFilters()
            .CountAsync(o => o.Number.StartsWith("DRAFT-"), cancellationToken);

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            Number = $"DRAFT-{draftCount + 1:D3}",
            Type = type,
            Status = OrderStatus.Draft,
            HubId = hubId,
            ScheduledAt = scheduledAt,
            DestinationCity = destinationCity,
            DestinationRegion = destinationRegion,
            CreatedByUserId = createdByUserId,
            PrimaryReference = primaryReference,
            CreatedAt = DateTimeOffset.UtcNow,
            NextActionLabel = "Continue editing",
            TimelineEntries =
            {
                new OrderTimelineEntry
                {
                    Id = Guid.NewGuid(),
                    Kind = "Status",
                    Text = "DRAFT",
                    CreatedAt = DateTimeOffset.UtcNow
                }
            }
        };

        dbContext.Orders.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderCreatedData(entity.Id, entity.Number, entity.Type, entity.Status);
    }
}