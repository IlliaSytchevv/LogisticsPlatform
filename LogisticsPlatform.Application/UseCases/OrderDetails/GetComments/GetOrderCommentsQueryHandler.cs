using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetComments;

public sealed class GetOrderCommentsQueryHandler(IOrderDetailsRepository orderDetailsRepository)
    : IQueryHandler<GetOrderCommentsQuery, Result<IReadOnlyList<OrderCommentResponse>>>
{
    public async Task<Result<IReadOnlyList<OrderCommentResponse>>> Handle(
        GetOrderCommentsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(query.OrderId, cancellationToken))
            return Result<IReadOnlyList<OrderCommentResponse>>.NotFound();

        IReadOnlyList<OrderCommentData> data = await orderDetailsRepository.GetCommentsAsync(
            query.OrderId,
            cancellationToken);

        return Result.Success<IReadOnlyList<OrderCommentResponse>>(
            data.Select(OrderDetailsMapper.ToResponse).ToList());
    }
}
