using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupply;

public sealed class UpdateOrderSupplyCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<UpdateOrderSupplyCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        UpdateOrderSupplyCommand command,
        CancellationToken cancellationToken)
    {
        OrderSupplyData? data = await orderDetailsRepository.UpdateSupplyAsync(
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
