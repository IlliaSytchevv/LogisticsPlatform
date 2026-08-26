using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationComments;

public sealed record GetOperationCommentsQuery(Guid OrderId, Guid OperationId)
    : IQuery<Result<IReadOnlyList<OrderCommentResponse>>>;
