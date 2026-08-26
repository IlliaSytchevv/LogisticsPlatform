using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;

public sealed record AddOrderSupplyCommand(
    Guid OrderId,
    string Sku,
    string Name,
    string Category,
    int Quantity,
    long UnitPriceCents) : ICommand<Result<OrderSupplyResponse>>;
