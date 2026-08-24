using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupplyQuantity;

public sealed class UpdateOrderSupplyQuantityCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<UpdateOrderSupplyQuantityCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        UpdateOrderSupplyQuantityCommand command,
        CancellationToken cancellationToken)
    {
        OrderSupplyData? data = await orderDetailsRepository.UpdateSupplyQuantityAsync(
            command.OrderId,
            command.SupplyId,
            command.Quantity,
            cancellationToken);

        if (data is null)
            return Result<OrderSupplyResponse>.NotFound();

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
