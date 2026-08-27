using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.ReleaseLock;

public sealed record ReleaseOrderEditLockCommand(Guid OrderId, Guid UserId)
    : ICommand<Result>;
