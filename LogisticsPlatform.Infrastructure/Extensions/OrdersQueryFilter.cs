using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Infrastructure.Repositories;

internal static class OrdersQueryFilter
{
    public static IQueryable<Order> Apply(IQueryable<Order> query, OrdersListFilter filter)
    {
        if (filter.HubId.HasValue)
            query = query.Where(o => o.HubId == filter.HubId.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(o => o.ScheduledAt >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(o => o.ScheduledAt <= filter.DateTo.Value);

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.Q))
        {
            string q = filter.Q.Trim();
            query = query.Where(o =>
                o.Number.Contains(q)
                || o.Hub.Name.Contains(q)
                || (o.Carrier != null && o.Carrier.Name.Contains(q))
                || o.CreatedByUser.DisplayName.Contains(q)
                || o.SubOrders.Any(s => s.Reference.Contains(q) || s.Number.Contains(q)));
        }

        if (filter.Tab.HasValue)
        {
            query = filter.Tab.Value switch
            {
                OrderListTab.CrossDock => query.Where(o => o.Type == OrderType.CrossDock),
                OrderListTab.Consolidation => query.Where(o => o.Type == OrderType.Consolidation),
                OrderListTab.Alerts => query.Where(o => o.HasAlert || o.Status == OrderStatus.Alert),
                OrderListTab.Drafts => query.Where(o => o.Status == OrderStatus.Draft),
                _ => query
            };
        }

        return query;
    }
}
