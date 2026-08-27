using LogisticsPlatform.Application;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Services;
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
            OrderStatusLabels.Format(data.Status),
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
            data.AwaitingClientAction,
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
            data.Supplies.Sum(s => s.LineTotalCents),
            data.IsPaid);
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
            data.AppliedAt,
            data.CommentCount,
            data.PhotoCount);

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
            $"{ApiPaths.V1Orders}/{data.OrderId}/warehouse-photos/{data.Id}");

    public static OrderOperationPhotoResponse ToResponse(OrderOperationPhotoData data) =>
        new(
            data.Id,
            data.FileName,
            data.ContentType,
            $"{ApiPaths.V1Orders}/{data.OrderId}/operations/{data.OperationId}/photos/{data.Id}");

    public static OrderCommentResponse ToResponse(OrderCommentData data) =>
        new(data.Id, data.Text, data.AuthorName, data.CreatedAt);

    public static OrderCommentResponse ToResponse(OrderOperationCommentData data) =>
        new(data.Id, data.Text, data.AuthorName, data.CreatedAt);

    public static OrderTimelineEntryResponse ToResponse(OrderTimelineEntryData data) =>
        new(data.Id, data.Kind, ResolveTimelineText(data), data.AuthorName, data.CreatedAt);

    private static string ResolveTimelineText(OrderTimelineEntryData data)
    {
        if (string.Equals(data.Kind, "Status", StringComparison.OrdinalIgnoreCase)
            && data.NewStatus is { } newStatus)
        {
            return OrderStatusLabels.FormatTransition(data.PreviousStatus, newStatus);
        }

        return data.Text;
    }

    private static string FormatOperationType(OrderOperationType type) => type switch
    {
        OrderOperationType.Unloading => "Unloading",
        OrderOperationType.Disposal => "DISPOSAL",
        OrderOperationType.Restack => "Restack",
        OrderOperationType.Loading => "Loading",
        _ => type.ToString()
    };

    private static string FormatType(OrderType type) => type switch
    {
        OrderType.Consolidation => "CONSOLIDATION",
        OrderType.CrossDock => "CROSS-DOCK",
        _ => type.ToString().ToUpperInvariant()
    };

    public static string FormatStatus(OrderStatus status) => OrderStatusLabels.Format(status);

    public static string? ToServicesCsv(IReadOnlyList<string>? services) =>
        services is { Count: > 0 }
            ? string.Join(",", services.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
            : null;
}
