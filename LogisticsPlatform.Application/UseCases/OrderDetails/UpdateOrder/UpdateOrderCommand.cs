using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed record UpdateOrderCommand(
    Guid OrderId,
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
    string? AlertReason) : ICommand<Result<UpdateOrderResponse>>;
