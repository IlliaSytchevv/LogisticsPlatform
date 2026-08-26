using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOrderDetails;

public sealed class GetOrderDetailsQueryHandler(IOrderDetailsQueryRepository orderDetailsQueryRepository)
    : IQueryHandler<GetOrderDetailsQuery, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(
        GetOrderDetailsQuery query,
        CancellationToken cancellationToken)
    {
        OrderDetailsData? data = await orderDetailsQueryRepository.GetDetailsAsync(query.OrderId, cancellationToken);
        if (data is null)
            return Result<OrderDetailsResponse>.NotFound();

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
