using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupplyQuantity;

public sealed class UpdateOrderSupplyQuantityCommandHandler(
    IOrderPaymentsRepository orderPaymentsRepository,
    IOrderSuppliesRepository orderSuppliesRepository)
    : ICommandHandler<UpdateOrderSupplyQuantityCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        UpdateOrderSupplyQuantityCommand command,
        CancellationToken cancellationToken)
    {
        if (await orderPaymentsRepository.HasPaidAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.Conflict("Cannot modify supplies on a paid order.");

        OrderSupplyData? data = await orderSuppliesRepository.UpdateSupplyQuantityAsync(
            command.OrderId,
            command.SupplyId,
            command.Quantity,
            cancellationToken);

        if (data is null)
            return Result<OrderSupplyResponse>.NotFound();

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
