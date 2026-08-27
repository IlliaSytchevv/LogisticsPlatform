using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupply;

public sealed class UpdateOrderSupplyCommandHandler(
    IOrderPaymentsRepository orderPaymentsRepository,
    IOrderSuppliesRepository orderSuppliesRepository)
    : ICommandHandler<UpdateOrderSupplyCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        UpdateOrderSupplyCommand command,
        CancellationToken cancellationToken)
    {
        if (await orderPaymentsRepository.HasPaidAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.Conflict("Cannot modify supplies on a paid order.");

        OrderSupplyData? data = await orderSuppliesRepository.UpdateSupplyAsync(
            command.OrderId,
            command.SupplyId,
            command.Sku,
            command.Name,
            command.Category,
            command.Quantity,
            command.UnitPriceCents,
            cancellationToken);

        if (data is null)
            return Result<OrderSupplyResponse>.NotFound();

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
