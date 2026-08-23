using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhotos;

public sealed class GetOperationPhotosQueryHandler(IOrderDetailsRepository orderDetailsRepository)
    : IQueryHandler<GetOperationPhotosQuery, Result<IReadOnlyList<OrderOperationPhotoResponse>>>
{
    public async Task<Result<IReadOnlyList<OrderOperationPhotoResponse>>> Handle(
        GetOperationPhotosQuery query,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.OperationExistsAsync(query.OrderId, query.OperationId, cancellationToken))
            return Result<IReadOnlyList<OrderOperationPhotoResponse>>.NotFound();

        IReadOnlyList<OrderOperationPhotoData> data = await orderDetailsRepository.GetOperationPhotosAsync(
            query.OrderId,
            query.OperationId,
            cancellationToken);

        return Result.Success<IReadOnlyList<OrderOperationPhotoResponse>>(
            data.Select(OrderDetailsMapper.ToResponse).ToList());
    }
}
