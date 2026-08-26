using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationComments;

public sealed class GetOperationCommentsQueryHandler(IOrderOperationsRepository orderOperationsRepository)
    : IQueryHandler<GetOperationCommentsQuery, Result<IReadOnlyList<OrderCommentResponse>>>
{
    public async Task<Result<IReadOnlyList<OrderCommentResponse>>> Handle(
        GetOperationCommentsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await orderOperationsRepository.OperationExistsAsync(
                query.OrderId,
                query.OperationId,
                cancellationToken))
            return Result<IReadOnlyList<OrderCommentResponse>>.NotFound();

        IReadOnlyList<OrderOperationCommentData> data =
            await orderOperationsRepository.GetOperationCommentsAsync(query.OperationId, cancellationToken);

        return Result.Success<IReadOnlyList<OrderCommentResponse>>(
            data.Select(OrderDetailsMapper.ToResponse).ToList());
    }
}
