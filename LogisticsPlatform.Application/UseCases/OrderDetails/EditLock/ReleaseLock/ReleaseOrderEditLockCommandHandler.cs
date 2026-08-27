using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.ReleaseLock;

public sealed class ReleaseOrderEditLockCommandHandler(IOrderEditLock orderEditLock)
    : ICommandHandler<ReleaseOrderEditLockCommand, Result>
{
    public async Task<Result> Handle(
        ReleaseOrderEditLockCommand command,
        CancellationToken cancellationToken)
    {
        await orderEditLock.ReleaseAsync(
            command.OrderId,
            command.UserId,
            cancellationToken);

        return Result.Success();
    }
}