using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record UpdateOrderRequest(
    string? CustomerName,
    string? PrimaryReference,
    int? DeclaredQty,
    int? ActualQty,
    string? TrailerType,
    string? Phone,
    string? TruckNumber,
    string? TrailerNumber,
    string? DockCode,
    string? DockBay,
    string? WarehouseNote,
    string? StockStatusLabel,
    string? LoadingStatusLabel,
    OrderStatus? Status);
