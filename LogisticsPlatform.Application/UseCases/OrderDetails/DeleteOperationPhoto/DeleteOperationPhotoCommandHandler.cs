using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperationPhoto;

public sealed class DeleteOperationPhotoCommandHandler(
    IOrderOperationsRepository orderOperationsRepository,
    IPhotoBlobStore photoBlobStore)
    : ICommandHandler<DeleteOperationPhotoCommand, Result>
{
    public async Task<Result> Handle(DeleteOperationPhotoCommand command, CancellationToken cancellationToken)
    {
        if (!await orderOperationsRepository.OperationExistsAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken))
            return Result.NotFound();

        string? storageKey = await orderOperationsRepository.SoftDeleteOperationPhotoAsync(
            command.OrderId,
            command.OperationId,
            command.PhotoId,
            cancellationToken);

        if (storageKey is null)
            return Result.NotFound();

        await photoBlobStore.DeleteAsync(storageKey, cancellationToken);
        return Result.Success();
    }
}
