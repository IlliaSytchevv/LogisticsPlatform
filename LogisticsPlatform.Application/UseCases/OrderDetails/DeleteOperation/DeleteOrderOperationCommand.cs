using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperation;

public sealed record DeleteOrderOperationCommand(
    Guid OrderId,
    Guid OperationId) : ICommand<Result>;
