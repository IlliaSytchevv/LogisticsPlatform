using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhoto;

public sealed class GetOperationPhotoQueryHandler(
    IOrderOperationsRepository orderOperationsRepository,
    IPhotoBlobStore photoBlobStore)
    : IQueryHandler<GetOperationPhotoQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetOperationPhotoQuery query,
        CancellationToken cancellationToken)
    {
        OrderOperationPhotoContentData? photo = await orderOperationsRepository.GetOperationPhotoContentAsync(
            query.OrderId,
            query.OperationId,
            query.PhotoId,
            cancellationToken);

        if (photo is null)
            return Result<OrderFileResponse>.NotFound();

        var file = new OrderFileResponse(
            photo.FileName,
            photo.ContentType,
            async (stream, ct) =>
            {
                await using Stream? source = await photoBlobStore.OpenReadAsync(photo.StorageKey, ct);
                if (source is null)
                    throw new FileNotFoundException("Photo blob is missing.", photo.StorageKey);

                await source.CopyToAsync(stream, ct);
            });

        return Result.Success(file);
    }
}
