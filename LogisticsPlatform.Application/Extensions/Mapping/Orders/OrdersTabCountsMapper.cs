using LogisticsPlatform.Application.DTO.Orders.FilterOptions;
using LogisticsPlatform.Application.DTO.Orders.TabCounts;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Extensions.Mapping.Orders;

public static class OrdersTabCountsMapper
{
    public static OrdersTabCountsResponse ToResponse(OrdersTabCountsData data) =>
        new(data.All, data.CrossDock, data.Consolidation, data.Alerts, data.Drafts);
}

public static class OrdersFilterOptionsMapper
{
    public static OrdersFilterOptionsResponse ToResponse(OrdersFilterOptionsData data) =>
        new(
            data.Hubs.Select(h => new OrderHubOptionResponse(h.Id, h.Name)).ToList(),
            Enum.GetValues<OrderStatus>()
                .Select(s => new OrderStatusOptionResponse(s.ToString(), StatusLabel(s)))
                .ToList());

    private static string StatusLabel(OrderStatus status) => status switch
    {
        OrderStatus.InProgress => "IN PROGRESS",
        OrderStatus.New => "NEW",
        OrderStatus.Alert => "ALERT",
        OrderStatus.Completed => "DONE",
        OrderStatus.Closed => "CLOSED",
        OrderStatus.Draft => "DRAFT",
        _ => status.ToString().ToUpperInvariant()
    };
}
