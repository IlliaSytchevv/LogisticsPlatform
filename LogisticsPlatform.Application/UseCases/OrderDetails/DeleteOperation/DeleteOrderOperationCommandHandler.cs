using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperation;

public sealed class DeleteOrderOperationCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderOperationsRepository orderOperationsRepository,
    IPhotoBlobStore photoBlobStore)
    : ICommandHandler<DeleteOrderOperationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderOperationCommand command, CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result.NotFound();

        IReadOnlyList<string>? storageKeys = await orderOperationsRepository.SoftDeleteOperationAsync(
            command.OrderId,
            command.OperationId,
            cancellationToken);

        if (storageKeys is null)
            return Result.NotFound();

        foreach (string key in storageKeys)
            await photoBlobStore.DeleteAsync(key, cancellationToken);

        return Result.Success();
    }
}
