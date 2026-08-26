using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Services;

public static class OrderStatusLabels
{
    public static string Format(OrderStatus status) => status switch
    {
        OrderStatus.InProgress => "IN PROGRESS",
        OrderStatus.New => "NEW",
        OrderStatus.Alert => "ALERT",
        OrderStatus.Completed => "DONE",
        OrderStatus.Closed => "CLOSED",
        OrderStatus.Draft => "DRAFT",
        _ => status.ToString().ToUpperInvariant()
    };

    public static string FormatTransition(OrderStatus? previousStatus, OrderStatus newStatus) =>
        previousStatus is null
            ? $"Created → {Format(newStatus)}"
            : $"{Format(previousStatus.Value)} → {Format(newStatus)}";
}
