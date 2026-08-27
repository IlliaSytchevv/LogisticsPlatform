using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record UpdateOrderRequest(
    string? Number,
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
    OrderStatus? Status,
    bool? AwaitingClientAction);
