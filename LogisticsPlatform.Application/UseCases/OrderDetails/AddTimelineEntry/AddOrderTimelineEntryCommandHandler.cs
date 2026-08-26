using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddTimelineEntry;

public sealed class AddOrderTimelineEntryCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderTimelineRepository orderTimelineRepository)
    : ICommandHandler<AddOrderTimelineEntryCommand, Result<OrderTimelineEntryResponse>>
{
    public async Task<Result<OrderTimelineEntryResponse>> Handle(
        AddOrderTimelineEntryCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderTimelineEntryResponse>.NotFound();

        OrderTimelineEntryData data = await orderTimelineRepository.AddTimelineEntryAsync(
            command.OrderId,
            "Manual",
            command.Text,
            command.AuthorName,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
