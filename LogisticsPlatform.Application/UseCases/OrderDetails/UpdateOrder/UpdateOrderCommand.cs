using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed record UpdateOrderCommand(
    Guid OrderId,
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
    OrderStatus? Status) : ICommand<Result<UpdateOrderResponse>>;
