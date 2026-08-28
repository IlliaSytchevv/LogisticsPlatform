using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.Acquire;

public sealed class AcquireOrderEditLockCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderEditLock orderEditLock)
    : ICommandHandler<AcquireOrderEditLockCommand, Result>
{
    public async Task<Result> Handle(
        AcquireOrderEditLockCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result.NotFound();

        bool acquired = await orderEditLock.TryAcquireAsync(
            command.OrderId,
            command.UserId,
            cancellationToken);

        return acquired
            ? Result.Success()
            : Result.Conflict("Order is being edited in another tab or device.");
    }
}
