using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;

public sealed class AddOrderOperationCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderOperationsRepository orderOperationsRepository)
    : ICommandHandler<AddOrderOperationCommand, Result<OrderOperationResponse>>
{
    public async Task<Result<OrderOperationResponse>> Handle(
        AddOrderOperationCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderOperationResponse>.NotFound();

        OrderOperationData data = await orderOperationsRepository.AddOperationAsync(
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
