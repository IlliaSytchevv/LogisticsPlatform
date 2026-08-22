using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;

public static class OrderDetailsMapper
{
    public static OrderDetailsResponse ToResponse(OrderDetailsData data)
    {
        int qtyDelta = (data.ActualQty ?? 0) - (data.DeclaredQty ?? 0);

        IReadOnlyList<string> services = string.IsNullOrWhiteSpace(data.ServicesCsv)
            ? []
            : data.ServicesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new OrderDetailsResponse(
            data.Id,
            data.Number,
            data.Type,
            FormatType(data.Type),
            data.Status,
            FormatStatus(data.Status),
            data.PrimaryReference,
            data.CustomerName,
            data.Phone,
            data.HubId,
            data.HubName,
            data.HubRegionCode,
            data.ScheduledAt,
            data.CarrierId,
            data.CarrierName,
            data.TrailerType,
            data.TruckNumber,
            data.TrailerNumber,
            data.AssignedToUserId,
            data.AssignedToUserName,
            services,
            data.StockStatusLabel,
            data.LoadingStatusLabel,
            data.HasAlert,
            data.AlertReason,
            new OrderAssignedDockResponse(
                data.HubName,
                data.DockCode,
                data.DockBay,
                data.TrailerNumber,
                data.DockAssignedAt,
                data.DockStatusLabel,
                data.HubDocks
                    .Select(d => new OrderHubDockResponse(
                        d.Code,
                        d.BayLabel,
                        string.Equals(d.Code, data.DockCode, StringComparison.OrdinalIgnoreCase)))
                    .ToList()),
            new OrderQtyBlockResponse(data.DeclaredQty, data.QuantityUnitLabel),
            new OrderQtyBlockResponse(data.ActualQty, data.QuantityUnitLabel),
            qtyDelta,
            new OrderWarehouseNoteResponse(
                data.WarehouseNote,
                data.WarehousePhotos
                    .Select(p => ToResponse(p))
                    .ToList()),
            data.Operations.Select(ToResponse).ToList(),
            data.Supplies.Select(ToResponse).ToList(),
            data.Supplies.Sum(s => s.LineTotalCents));
    }

    public static OrderOperationResponse ToResponse(OrderOperationData data) =>
        new(
            data.Id,
            data.Type,
            FormatOperationType(data.Type),
            data.Trailer,
            data.Quantity,
            data.Unit,
            data.UnitLabel,
            data.AppliedAt);

    public static OrderSupplyResponse ToResponse(OrderSupplyData data) =>
        new(
            data.Id,
            data.Sku,
            data.Name,
            data.Category,
            data.Quantity,
            data.UnitPriceCents,
            data.LineTotalCents);

    public static OrderWarehousePhotoResponse ToResponse(OrderWarehousePhotoData data) =>
        new(
            data.Id,
            data.FileName,
            data.ContentType,
            data.SortOrder,
            $"/api/orders/{data.OrderId}/warehouse-photos/{data.Id}");

    public static OrderCommentResponse ToResponse(OrderCommentData data) =>
        new(data.Id, data.Text, data.AuthorName, data.CreatedAt);

    public static string FormatOperationType(OrderOperationType type) => type switch
    {
        OrderOperationType.Unloading => "Unloading",
        OrderOperationType.Disposal => "DISPOSAL",
        OrderOperationType.Restack => "Restack",
        OrderOperationType.Loading => "Loading",
        _ => type.ToString()
    };

    public static string FormatType(OrderType type) => type switch
    {
        OrderType.Consolidation => "CONSOLIDATION",
        OrderType.CrossDock => "CROSS-DOCK",
        _ => type.ToString().ToUpperInvariant()
    };

    public static string FormatStatus(OrderStatus status) => status switch
    {
        OrderStatus.InProgress => "IN PROGRESS",
        OrderStatus.New => "NEW",
        OrderStatus.Alert => "ALERT",
        OrderStatus.Completed => "DONE",
        OrderStatus.Closed => "CLOSED",
        OrderStatus.Draft => "DRAFT",
        _ => status.ToString().ToUpperInvariant()
    };

    public static string? ToServicesCsv(IReadOnlyList<string>? services) =>
        services is { Count: > 0 }
            ? string.Join(",", services.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
            : null;
}
