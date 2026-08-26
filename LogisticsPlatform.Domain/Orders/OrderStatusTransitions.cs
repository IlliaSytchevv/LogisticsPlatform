using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.Orders;

public static class OrderStatusTransitions
{
    private static readonly IReadOnlyDictionary<OrderStatus, OrderStatus[]> Allowed =
        new Dictionary<OrderStatus, OrderStatus[]>
        {
            [OrderStatus.Draft] = [OrderStatus.New, OrderStatus.Closed],
            [OrderStatus.New] = [OrderStatus.InProgress, OrderStatus.Alert, OrderStatus.Closed],
            [OrderStatus.InProgress] = [OrderStatus.Alert, OrderStatus.Completed, OrderStatus.Closed],
            [OrderStatus.Alert] = [OrderStatus.InProgress, OrderStatus.Completed, OrderStatus.Closed],
            [OrderStatus.Completed] = [OrderStatus.Closed],
            [OrderStatus.Closed] = []
        };

    public static bool IsAllowed(OrderStatus from, OrderStatus to) =>
        from == to
        || (Allowed.TryGetValue(from, out OrderStatus[]? next) && next.Contains(to));
}
