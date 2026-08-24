using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupplyQuantity;

public sealed record UpdateOrderSupplyQuantityCommand(
    Guid OrderId,
    Guid SupplyId,
    int Quantity) : ICommand<Result<OrderSupplyResponse>>;
