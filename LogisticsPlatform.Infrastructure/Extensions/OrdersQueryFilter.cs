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

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim();
            query = query.Where(o =>
                o.Number.Contains(search)
                || o.Hub.Name.Contains(search)
                || (o.Carrier != null && o.Carrier.Name.Contains(search))
                || o.CreatedByUser.DisplayName.Contains(search)
                || o.SubOrders.Any(s => s.Reference.Contains(search) || s.Number.Contains(search)));
        }

        if (filter.Tab.HasValue)
            query = ApplyTab(query, filter.Tab.Value);

        return query;
    }

    private static IQueryable<Order> ApplyTab(IQueryable<Order> query, OrderListTab tab) =>
        tab switch
        {
            OrderListTab.Drafts => query.Where(o => o.Status == OrderStatus.Draft),
            OrderListTab.Alerts => query.Where(o =>
                o.Status != OrderStatus.Draft &&
                (o.HasAlert || o.Status == OrderStatus.Alert)),
            OrderListTab.CrossDock => query.Where(o =>
                o.Type == OrderType.CrossDock &&
                o.Status != OrderStatus.Draft &&
                !o.HasAlert &&
                o.Status != OrderStatus.Alert),
            OrderListTab.Consolidation => query.Where(o =>
                o.Type == OrderType.Consolidation &&
                o.Status != OrderStatus.Draft &&
                !o.HasAlert &&
                o.Status != OrderStatus.Alert),
            _ => query
        };
}