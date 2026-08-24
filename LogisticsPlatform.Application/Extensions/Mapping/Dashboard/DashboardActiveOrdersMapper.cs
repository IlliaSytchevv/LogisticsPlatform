using LogisticsPlatform.Application.Models.Dashboard;
using LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Extensions.Mapping.Dashboard;

public static class DashboardActiveOrdersMapper
{
    public static DashboardActiveOrdersResponse ToResponse(
        IReadOnlyList<ActiveOrderData> orders,
        DateTimeOffset now)
    {
        return new(orders.Select(o => ToCard(o, now)).ToList());
    }

    private static OrderCardResponse ToCard(ActiveOrderData order, DateTimeOffset now) =>
        new(
            order.Id,
            order.Number,
            order.Type,
            order.Status,
            TypeLabel(order.Type),
            StatusLabel(order.Status),
            order.HasAlert,
            new OrderCreatedByResponse(order.CreatedByName, order.CreatedByInitials, order.CreatedByRole),
            order.SubOrders.Select(ToReference).ToList(),
            order.Hub,
            order.ScheduledAt,
            QuantityDisplay(order.DeclaredQty, order.ActualQty, order.QuantityLines),
            order.DeclaredQty,
            order.ActualQty,
            order.CarrierName ?? "—",
            Destination(order.DestinationCity, order.DestinationRegion, order.DestinationNote),
            order.Type == OrderType.Consolidation ? order.TrailersConsolidated : null,
            new OrderNextActionResponse(
                order.NextActionLabel ?? string.Empty,
                order.NextActionKind,
                order.NextActionDueAt.HasValue
                    ? Math.Max(0, (int)(order.NextActionDueAt.Value - now).TotalSeconds)
                    : null,
                order.HasAlert || order.NextActionKind == NextActionKind.UploadPhoto,
                order.NextActionAmountCents,
                order.NextActionDocumentNumber));

    private static OrderReferenceResponse ToReference(ActiveOrderSubOrderData subOrder) =>
        new(subOrder.Number, subOrder.Reference, $"{subOrder.PalletCount} pallets", 
            subOrder.HasMissingPhoto ? "missing photo" : null);

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
        OrderStatus.Completed => "COMPLETED",
        OrderStatus.Closed => "CLOSED",
        OrderStatus.Draft => "DRAFT",
        _ => status.ToString().ToUpperInvariant()
    };

    private static string Destination(string city, string region, string? note)
    {
        string result = $"{city}, {region}";
        
        return string.IsNullOrWhiteSpace(note) ? result : $"{result} · {note}";
    }

    private static string QuantityDisplay(
        int? declared,
        int? actual,
        IReadOnlyList<ActiveOrderQuantityLineData> lines)
    {
        if (declared.HasValue && actual.HasValue && declared != actual)
        {
            return $"{actual} / {declared}+";
        }

        if (lines.Count > 0)
        {
            return string.Join(" + ",
                lines.Select(x =>
                    x.Unit == PalletUnit.XL ? $"{x.Count} XL" : x.Count.ToString()));
        }

        return actual?.ToString() ?? declared?.ToString() ?? "—";
    }
}