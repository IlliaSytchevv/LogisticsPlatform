using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;

public sealed class AddOrderOperationCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddOrderOperationCommand, Result<OrderOperationResponse>>
{
    public async Task<Result<OrderOperationResponse>> Handle(
        AddOrderOperationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderOperationResponse>.NotFound();

        OrderOperationData data = await orderDetailsRepository.AddOperationAsync(
            command.OrderId,
            command.Type,
            command.Trailer,
            command.Quantity,
            command.Unit,
            command.UnitLabel,
            command.AppliedAt ?? DateTimeOffset.UtcNow,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
