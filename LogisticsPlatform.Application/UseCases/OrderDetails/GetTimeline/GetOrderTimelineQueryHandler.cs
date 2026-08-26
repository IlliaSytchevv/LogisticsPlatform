using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetTimeline;

public sealed class GetOrderTimelineQueryHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderTimelineRepository orderTimelineRepository)
    : IQueryHandler<GetOrderTimelineQuery, Result<IReadOnlyList<OrderTimelineEntryResponse>>>
{
    public async Task<Result<IReadOnlyList<OrderTimelineEntryResponse>>> Handle(
        GetOrderTimelineQuery query,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(query.OrderId, cancellationToken))
            return Result<IReadOnlyList<OrderTimelineEntryResponse>>.NotFound();

        IReadOnlyList<OrderTimelineEntryData> data = await orderTimelineRepository.GetTimelineAsync(
            query.OrderId,
            cancellationToken);

        return Result.Success<IReadOnlyList<OrderTimelineEntryResponse>>(
            data.Select(OrderDetailsMapper.ToResponse).ToList());
    }
}
