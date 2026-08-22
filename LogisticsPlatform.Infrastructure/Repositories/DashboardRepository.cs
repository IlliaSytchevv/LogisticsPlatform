using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Dashboard;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories;

public sealed class DashboardRepository(AppDbContext dbContext) : IDashboardRepository
{
    private static readonly OrderStatus[] ActiveStatuses =
    [
        OrderStatus.New,
        OrderStatus.InProgress,
        OrderStatus.Alert
    ];

    public async Task<DashboardMetricsData> GetMetricsAsync(CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset startOfWeek = StartOfWeek(now);
        DateTimeOffset last30Days = now.AddDays(-30);
        DateTimeOffset previous30Days = now.AddDays(-60);

        var metrics = await dbContext.Orders
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ActiveOrders = g.Count(o => ActiveStatuses.Contains(o.Status)),
                ActiveOrdersThisWeek = g.Count(o => ActiveStatuses.Contains(o.Status) && o.CreatedAt >= startOfWeek),
                CompletedLast30Days = g.Count(o => o.Status == OrderStatus.Completed && o.CompletedAt >= last30Days),
                CompletedPrevious30Days = g.Count(o =>
                    o.Status == OrderStatus.Completed
                    && o.CompletedAt >= previous30Days
                    && o.CompletedAt < last30Days),
                AwaitingClientAction = g.Count(o => o.AwaitingClientAction),
                Alerts = g.Count(o => o.HasAlert),
                NeedAttention = g.Count(o => o.AwaitingClientAction || o.HasAlert)
            })
            .SingleOrDefaultAsync(cancellationToken);

        List<AlertSampleData> alertSamples = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.HasAlert)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new AlertSampleData(o.Number, o.AlertReason ?? "alert"))
            .ToListAsync(cancellationToken);

        return new DashboardMetricsData(
            metrics?.ActiveOrders ?? 0,
            metrics?.ActiveOrdersThisWeek ?? 0,
            metrics?.CompletedLast30Days ?? 0,
            metrics?.CompletedPrevious30Days ?? 0,
            metrics?.NeedAttention ?? 0,
            metrics?.AwaitingClientAction ?? 0,
            metrics?.Alerts ?? 0,
            alertSamples);
    }

    public async Task<IReadOnlyList<ActiveOrderData>> GetActiveOrdersAsync(
        int take,
        CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(o => ActiveStatuses.Contains(o.Status))
            .OrderByDescending(o => o.ScheduledAt)
            .Take(take)
            .Select(o => new
            {
                o.Id,
                o.Number,
                o.Type,
                o.Status,
                o.HasAlert,
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
                o.TrailersConsolidated,
                o.NextActionLabel,
                o.NextActionKind,
                o.NextActionDueAt,
                o.NextActionAmountCents,
                o.NextActionDocumentNumber
            })
            .ToListAsync(cancellationToken);

        if (orders.Count == 0)
            return [];

        Guid[] orderIds = orders.Select(o => o.Id).ToArray();

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
                g => (IReadOnlyList<ActiveOrderQuantityLineData>)g
                    .Select(x => new ActiveOrderQuantityLineData(x.Unit, x.Count))
                    .ToList());

        var subOrdersByOrder = subOrders
            .GroupBy(x => x.OrderId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<ActiveOrderSubOrderData>)g
                    .Select(x => new ActiveOrderSubOrderData(
                        x.Number,
                        x.Reference,
                        x.PalletCount,
                        x.HasMissingPhoto))
                    .ToList());

        return orders
            .Select(o => new ActiveOrderData(
                o.Id,
                o.Number,
                o.Type,
                o.Status,
                o.HasAlert,
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
                o.TrailersConsolidated,
                o.NextActionLabel,
                o.NextActionKind,
                o.NextActionDueAt,
                o.NextActionAmountCents,
                o.NextActionDocumentNumber,
                quantitiesByOrder.GetValueOrDefault(o.Id, []),
                subOrdersByOrder.GetValueOrDefault(o.Id, [])))
            .ToList();
    }

    public async Task<DashboardActivityData> GetActivityAsync(
        DateTimeOffset rangeStart,
        DateTimeOffset previousStart,
        CancellationToken cancellationToken)
    {
        int previousCompleted = await dbContext.Orders
            .AsNoTracking()
            .CountAsync(
                o => o.Status == OrderStatus.Completed
                     && o.CompletedAt >= previousStart
                     && o.CompletedAt < rangeStart,
                cancellationToken);

        List<CompletedActivityRow> currentRows = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Completed && o.CompletedAt >= rangeStart)
            .Select(o => new CompletedActivityRow(o.CompletedAt!.Value, o.SpendCents))
            .ToListAsync(cancellationToken);

        return new DashboardActivityData(currentRows, previousCompleted);
    }

    private static DateTimeOffset StartOfWeek(DateTimeOffset value)
    {
        int daysFromMonday = (7 + (value.DayOfWeek - DayOfWeek.Monday)) % 7;
        return new DateTimeOffset(value.UtcDateTime.Date.AddDays(-daysFromMonday), TimeSpan.Zero);
    }
}
