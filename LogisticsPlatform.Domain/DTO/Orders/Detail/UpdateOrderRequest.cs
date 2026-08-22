using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record UpdateOrderRequest(
    string? CustomerName,
    string? PrimaryReference,
    Guid? HubId,
    DateTimeOffset? ScheduledAt,
    int? DeclaredQty,
    int? ActualQty,
    string? TrailerType,
    Guid? CarrierId,
    string? Phone,
    string? TruckNumber,
    string? TrailerNumber,
    string? DockCode,
    string? DockBay,
    DateTimeOffset? DockAssignedAt,
    Guid? AssignedToUserId,
    string? WarehouseNote,
    string? StockStatusLabel,
    string? LoadingStatusLabel,
    IReadOnlyList<string>? Services,
    string? QuantityUnitLabel,
    string? DockStatusLabel,
    OrderStatus? Status,
    bool? HasAlert,
    string? AlertReason);
