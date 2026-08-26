using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.Acquire;

public sealed record AcquireOrderEditLockCommand(Guid OrderId, Guid UserId)
    : ICommand<Result>;
