using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;

public sealed class AddOrderSupplyCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddOrderSupplyCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        AddOrderSupplyCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.NotFound();

        OrderSupplyData data = await orderDetailsRepository.AddSupplyAsync(
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
