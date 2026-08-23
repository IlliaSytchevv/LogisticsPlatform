using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddTimelineEntry;

public sealed class AddOrderTimelineEntryCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddOrderTimelineEntryCommand, Result<OrderTimelineEntryResponse>>
{
    public async Task<Result<OrderTimelineEntryResponse>> Handle(
        AddOrderTimelineEntryCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderTimelineEntryResponse>.NotFound();

        OrderTimelineEntryData data = await orderDetailsRepository.AddTimelineEntryAsync(
            command.OrderId,
            "Manual",
            command.Text,
            command.AuthorName,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
