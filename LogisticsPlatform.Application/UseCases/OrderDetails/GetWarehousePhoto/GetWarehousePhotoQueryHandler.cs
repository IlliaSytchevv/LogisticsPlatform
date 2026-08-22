using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetWarehousePhoto;

public sealed class GetWarehousePhotoQueryHandler(IOrderDetailsRepository orderDetailsRepository)
    : IQueryHandler<GetWarehousePhotoQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetWarehousePhotoQuery query,
        CancellationToken cancellationToken)
    {
        OrderWarehousePhotoContentData? photo = await orderDetailsRepository.GetWarehousePhotoContentAsync(
            query.OrderId,
            query.PhotoId,
            cancellationToken);

        if (photo is null)
            return Result<OrderFileResponse>.NotFound();

        var file = new OrderFileResponse(
            photo.FileName,
            photo.ContentType,
            (stream, ct) => stream.WriteAsync(photo.Content, ct).AsTask());

        return Result.Success(file);
    }
}
