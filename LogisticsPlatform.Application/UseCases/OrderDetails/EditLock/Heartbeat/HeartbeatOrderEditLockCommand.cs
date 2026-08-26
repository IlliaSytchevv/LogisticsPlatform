using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.Heartbeat;

public sealed record HeartbeatOrderEditLockCommand(Guid OrderId, Guid UserId)
    : ICommand<Result>;
