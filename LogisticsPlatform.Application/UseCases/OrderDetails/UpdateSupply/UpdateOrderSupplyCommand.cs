using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupply;

public sealed record UpdateOrderSupplyCommand(
    Guid OrderId,
    Guid SupplyId,
    string Sku,
    string Name,
    string Category,
    int Quantity,
    long UnitPriceCents) : ICommand<Result<OrderSupplyResponse>>;
