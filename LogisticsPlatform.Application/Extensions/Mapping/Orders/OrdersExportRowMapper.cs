using System.Globalization;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Extensions.Mapping.Orders;

public static class OrdersExportRowMapper
{
    public static IReadOnlyList<object?> MapRow(OrderExportRowData row) =>
    [
        row.Number,
        FormatType(row.Type),
        FormatStatus(row.Status),
        row.Hub,
        row.ScheduledAt.ToString("u", CultureInfo.InvariantCulture),
        row.CarrierName ?? string.Empty,
        row.CreatedByName,
        row.CreatedByRole.ToString(),
        row.DeclaredQty,
        row.ActualQty,
        row.QuantityDisplay,
        row.References,
        row.NextActionLabel ?? string.Empty,
        row.HasAlert,
        row.AlertReason ?? string.Empty
    ];

    private static string FormatType(OrderType type) => type switch
    {
        OrderType.Consolidation => "CONSOLIDATION",
        OrderType.CrossDock => "CROSS-DOCK",
        _ => type.ToString().ToUpperInvariant()
    };

    private static string FormatStatus(OrderStatus status) => status switch
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