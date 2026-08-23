using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhoto;

public sealed class GetOperationPhotoQueryHandler(IOrderDetailsRepository orderDetailsRepository)
    : IQueryHandler<GetOperationPhotoQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetOperationPhotoQuery query,
        CancellationToken cancellationToken)
    {
        OrderOperationPhotoContentData? photo = await orderDetailsRepository.GetOperationPhotoContentAsync(
            query.OrderId,
            query.OperationId,
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
