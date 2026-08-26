using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed class AddWarehousePhotoCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderWarehousePhotosRepository orderWarehousePhotosRepository,
    IPhotoBlobStore photoBlobStore)
    : ICommandHandler<AddWarehousePhotoCommand, Result<OrderWarehousePhotoResponse>>
{
    public async Task<Result<OrderWarehousePhotoResponse>> Handle(
        AddWarehousePhotoCommand command,
        CancellationToken cancellationToken)
    {
        if (!ImageContentTypeDetector.TryDetect(command.Content, out string contentType))
        {
            return Result<OrderWarehousePhotoResponse>.Invalid(
            [
                new ValidationError("Content", "Only jpeg, png, webp and gif images are allowed.")
            ]);
        }

        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderWarehousePhotoResponse>.NotFound();

        Guid photoId = Guid.NewGuid();
        string storageKey = PhotoStorageKeys.ForWarehouse(command.OrderId, photoId, contentType);

        await photoBlobStore.SaveAsync(storageKey, command.Content, cancellationToken);

        try
        {
            OrderWarehousePhotoData data = await orderWarehousePhotosRepository.AddWarehousePhotoAsync(
                command.OrderId,
                photoId,
                command.FileName,
                contentType,
                storageKey,
                cancellationToken);

            return Result.Success(OrderDetailsMapper.ToResponse(data));
        }
        catch
        {
            await photoBlobStore.DeleteAsync(storageKey, cancellationToken);
            throw;
        }
    }
}
