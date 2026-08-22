using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteSupply;

public sealed record DeleteOrderSupplyCommand(
    Guid OrderId,
    Guid SupplyId) : ICommand<Result>;
