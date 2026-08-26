using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Domain.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed class UpdateOrderCommandHandler(IOrderPatchRepository orderPatchRepository)
    : ICommandHandler<UpdateOrderCommand, Result<UpdateOrderResponse>>
{
    public async Task<Result<UpdateOrderResponse>> Handle(
        UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Status is { } nextStatus)
        {
            OrderStatus? currentStatus =
                await orderPatchRepository.GetStatusAsync(command.OrderId, cancellationToken);

            if (currentStatus is null)
                return Result<UpdateOrderResponse>.NotFound();

            if (!OrderStatusTransitions.IsAllowed(currentStatus.Value, nextStatus))
            {
                return Result<UpdateOrderResponse>.Invalid(
                [
                    new ValidationError(
                        nameof(command.Status),
                        $"Transition from {currentStatus.Value} to {nextStatus} is not allowed.")
                ]);
            }
        }

        var patch = new OrderDetailPatchData(
            command.OrderId,
            command.CustomerName,
            command.PrimaryReference,
            command.DeclaredQty,
            command.ActualQty,
            command.TrailerType,
            command.Phone,
            command.TruckNumber,
            command.TrailerNumber,
            command.DockCode,
            command.DockBay,
            command.WarehouseNote,
            command.StockStatusLabel,
            command.LoadingStatusLabel,
            command.Status);

        bool updated = await orderPatchRepository.PatchOrderAsync(patch, cancellationToken);
        if (!updated)
            return Result<UpdateOrderResponse>.NotFound();

        return Result.Success(new UpdateOrderResponse(command.OrderId));
    }
}
