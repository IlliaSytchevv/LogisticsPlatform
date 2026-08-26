using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Services;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;

public sealed class AddOperationPhotoCommandHandler(
    IOrderOperationsRepository orderOperationsRepository,
    IPhotoBlobStore photoBlobStore)
    : ICommandHandler<AddOperationPhotoCommand, Result<OrderOperationPhotoResponse>>
{
    public async Task<Result<OrderOperationPhotoResponse>> Handle(
        AddOperationPhotoCommand command,
        CancellationToken cancellationToken)
    {
        if (!ImageContentTypeDetector.TryDetect(command.Content, out string contentType))
        {
            return Result<OrderOperationPhotoResponse>.Invalid(
            [
                new ValidationError("Content", "Only jpeg, png, webp and gif images are allowed.")
            ]);
        }

        if (!await orderOperationsRepository.OperationExistsAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken))
            return Result<OrderOperationPhotoResponse>.NotFound();

        Guid photoId = Guid.NewGuid();
        string storageKey = PhotoStorageKeys.ForOperation(command.OperationId, photoId, contentType);

        await photoBlobStore.SaveAsync(storageKey, command.Content, cancellationToken);

        try
        {
            OrderOperationPhotoData data = await orderOperationsRepository.AddOperationPhotoAsync(
                command.OrderId,
                command.OperationId,
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
