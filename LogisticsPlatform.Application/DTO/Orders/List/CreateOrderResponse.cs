using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Orders.List;

public sealed record CreateOrderResponse(
    Guid Id,
    string Number,
    OrderType Type,
    OrderStatus Status);
