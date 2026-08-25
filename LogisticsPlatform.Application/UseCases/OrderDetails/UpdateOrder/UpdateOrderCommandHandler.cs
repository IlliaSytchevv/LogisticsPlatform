using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed class UpdateOrderCommandHandler(
    IOrderDetailsRepository orderDetailsRepository,
    INotificationsFeedCacheInvalidator notificationsFeedCacheInvalidator)
    : ICommandHandler<UpdateOrderCommand, Result<UpdateOrderResponse>>
{
    public async Task<Result<UpdateOrderResponse>> Handle(
        UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
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

        bool updated = await orderDetailsRepository.PatchOrderAsync(patch, cancellationToken);
        if (!updated)
            return Result<UpdateOrderResponse>.NotFound();

        await notificationsFeedCacheInvalidator.InvalidateAsync(cancellationToken);

        return Result.Success(new UpdateOrderResponse(command.OrderId));
    }
}
