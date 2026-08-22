using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Extensions.Mapping.Orders;

public static class OrdersListMapper
{
    public static OrdersListResponse ToResponse(OrdersListData data, int page, int pageSize, DateTimeOffset now) =>
        new(
            data.TotalCount,
            page,
            pageSize,
            data.Items.Select(o => ToItem(o, now)).ToList());

    private static OrderListItemResponse ToItem(OrderListItemData order, DateTimeOffset now) =>
        new(
            order.Id,
            order.Number,
            order.Type,
            order.Status,
            TypeLabel(order.Type),
            StatusLabel(order.Status),
            Subtitle(order),
            ReferenceSummary(order.SubOrders),
            order.HasAlert,
            order.AlertReason,
            order.Status == OrderStatus.Draft,
            new OrderListCreatedByResponse(order.CreatedByName, order.CreatedByInitials, order.CreatedByRole),
            order.SubOrders.Select(ToReference).ToList(),
            order.Hub,
            order.ScheduledAt,
            QuantityDisplay(order.DeclaredQty, order.ActualQty, order.QuantityLines),
            order.DeclaredQty,
            order.ActualQty,
            order.CarrierName ?? "—",
            new OrderListNextActionResponse(
                order.NextActionLabel ?? string.Empty,
                order.NextActionKind,
                order.NextActionDueAt.HasValue
                    ? Math.Max(0, (int)(order.NextActionDueAt.Value - now).TotalSeconds)
                    : null,
                order.HasAlert || order.NextActionKind == NextActionKind.UploadPhoto,
                order.NextActionAmountCents,
                order.NextActionDocumentNumber));

    private static OrderListReferenceResponse ToReference(OrderListSubOrderData subOrder) =>
        new(
            subOrder.Number,
            subOrder.Reference,
            $"{subOrder.PalletCount} pallets",
            subOrder.HasMissingPhoto ? "missing photo" : null);

    private static string ReferenceSummary(IReadOnlyList<OrderListSubOrderData> subOrders) =>
        subOrders.Count switch
        {
            0 => "—",
            1 => subOrders[0].Reference,
            _ => subOrders.Count.ToString()
        };

    private static string Subtitle(OrderListItemData order)
    {
        if (order.Status == OrderStatus.Draft)
            return "incomplete";

        if (order.Type == OrderType.Consolidation)
            return $"Consolidation - {order.SubOrders.Count} sub";

        return string.IsNullOrWhiteSpace(order.DestinationNote)
            ? "Cross-Dock"
            : $"Cross-Dock - {order.DestinationNote}";
    }

    private static string TypeLabel(OrderType type) => type switch
    {
        OrderType.Consolidation => "CONSOLIDATION",
        OrderType.CrossDock => "CROSS-DOCK",
        _ => type.ToString().ToUpperInvariant()
    };

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

    private static string QuantityDisplay(
        int? declared,
        int? actual,
        IReadOnlyList<OrderListQuantityLineData> lines)
    {
        if (declared.HasValue && actual.HasValue && declared != actual)
            return $"{actual} / {declared}+";

        if (lines.Count > 0)
        {
            return string.Join(
                " + ",
                lines.Select(x =>
                    x.Unit == PalletUnit.XL
                        ? $"{x.Count} XL"
                        : x.Unit == PalletUnit.Standard
                            ? $"{x.Count} Std"
                            : x.Count.ToString()));
        }

        return actual?.ToString() ?? declared?.ToString() ?? "—";
    }
}