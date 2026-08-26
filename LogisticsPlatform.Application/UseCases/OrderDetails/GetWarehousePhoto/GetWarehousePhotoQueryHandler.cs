using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetWarehousePhoto;

public sealed class GetWarehousePhotoQueryHandler(
    IOrderWarehousePhotosRepository orderWarehousePhotosRepository,
    IPhotoBlobStore photoBlobStore)
    : IQueryHandler<GetWarehousePhotoQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetWarehousePhotoQuery query,
        CancellationToken cancellationToken)
    {
        OrderWarehousePhotoContentData? photo = await orderWarehousePhotosRepository.GetWarehousePhotoContentAsync(
            query.OrderId,
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
