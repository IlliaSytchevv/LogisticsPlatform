using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;

public sealed class AddOrderSupplyCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderPaymentsRepository orderPaymentsRepository,
    IOrderSuppliesRepository orderSuppliesRepository)
    : ICommandHandler<AddOrderSupplyCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        AddOrderSupplyCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.NotFound();

        if (await orderPaymentsRepository.HasPaidAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.Conflict("Cannot modify supplies on a paid order.");

        OrderSupplyData data = await orderSuppliesRepository.AddSupplyAsync(
            command.OrderId,
            command.Sku,
            command.Name,
            command.Category,
            command.Quantity,
            command.UnitPriceCents,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
