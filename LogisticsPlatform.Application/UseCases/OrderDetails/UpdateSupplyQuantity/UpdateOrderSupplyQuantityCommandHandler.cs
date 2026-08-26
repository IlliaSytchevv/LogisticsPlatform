using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupplyQuantity;

public sealed class UpdateOrderSupplyQuantityCommandHandler(IOrderSuppliesRepository orderSuppliesRepository)
    : ICommandHandler<UpdateOrderSupplyQuantityCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        UpdateOrderSupplyQuantityCommand command,
        CancellationToken cancellationToken)
    {
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
