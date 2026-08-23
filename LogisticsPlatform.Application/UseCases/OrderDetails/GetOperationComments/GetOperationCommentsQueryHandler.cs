using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationComments;

public sealed class GetOperationCommentsQueryHandler(IOrderDetailsRepository orderDetailsRepository)
    : IQueryHandler<GetOperationCommentsQuery, Result<IReadOnlyList<OrderCommentResponse>>>
{
    public async Task<Result<IReadOnlyList<OrderCommentResponse>>> Handle(
        GetOperationCommentsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.OperationExistsAsync(
                query.OrderId,
                query.OperationId,
                cancellationToken))
            return Result<IReadOnlyList<OrderCommentResponse>>.NotFound();

        IReadOnlyList<OrderOperationCommentData> data =
            await orderDetailsRepository.GetOperationCommentsAsync(query.OperationId, cancellationToken);

        return Result.Success<IReadOnlyList<OrderCommentResponse>>(
            data.Select(OrderDetailsMapper.ToResponse).ToList());
    }
}
