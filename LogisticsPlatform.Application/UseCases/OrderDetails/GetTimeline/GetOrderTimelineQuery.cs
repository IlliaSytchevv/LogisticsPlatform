using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetTimeline;

public sealed record GetOrderTimelineQuery(Guid OrderId)
    : IQuery<Result<IReadOnlyList<OrderTimelineEntryResponse>>>;
