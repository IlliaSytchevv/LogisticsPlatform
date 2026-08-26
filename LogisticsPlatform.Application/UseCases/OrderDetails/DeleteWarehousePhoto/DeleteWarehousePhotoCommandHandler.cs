using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteWarehousePhoto;

public sealed class DeleteWarehousePhotoCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderWarehousePhotosRepository orderWarehousePhotosRepository,
    IPhotoBlobStore photoBlobStore)
    : ICommandHandler<DeleteWarehousePhotoCommand, Result>
{
    public async Task<Result> Handle(DeleteWarehousePhotoCommand command, CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result.NotFound();

        string? storageKey = await orderWarehousePhotosRepository.SoftDeleteWarehousePhotoAsync(
            command.OrderId,
            command.PhotoId,
            cancellationToken);

        if (storageKey is null)
            return Result.NotFound();

        await photoBlobStore.DeleteAsync(storageKey, cancellationToken);
        return Result.Success();
    }
}
