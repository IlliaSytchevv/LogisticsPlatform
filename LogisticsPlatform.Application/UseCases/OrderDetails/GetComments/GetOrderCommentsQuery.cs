using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetComments;

public sealed record GetOrderCommentsQuery(Guid OrderId)
    : IQuery<Result<IReadOnlyList<OrderCommentResponse>>>;
