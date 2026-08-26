using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.Heartbeat;

public sealed class HeartbeatOrderEditLockCommandHandler(IOrderEditLock orderEditLock)
    : ICommandHandler<HeartbeatOrderEditLockCommand, Result>
{
    public async Task<Result> Handle(
        HeartbeatOrderEditLockCommand command,
        CancellationToken cancellationToken)
    {
        bool ok = await orderEditLock.HeartbeatAsync(
            command.OrderId,
            command.UserId,
            cancellationToken);

        return ok
            ? Result.Success()
            : Result.Conflict("Edit lock was lost. Close and try again.");
    }
}
