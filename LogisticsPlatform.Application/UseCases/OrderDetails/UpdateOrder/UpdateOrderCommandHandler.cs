using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
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
            SetCustomerName: command.CustomerName is not null,
            command.PrimaryReference,
            SetPrimaryReference: command.PrimaryReference is not null,
            command.HubId,
            command.ScheduledAt,
            command.DeclaredQty,
            SetDeclaredQty: command.DeclaredQty is not null,
            command.ActualQty,
            SetActualQty: command.ActualQty is not null,
            command.TrailerType,
            SetTrailerType: command.TrailerType is not null,
            command.CarrierId,
            SetCarrierId: command.CarrierId is not null,
            command.Phone,
            SetPhone: command.Phone is not null,
            command.TruckNumber,
            SetTruckNumber: command.TruckNumber is not null,
            command.TrailerNumber,
            SetTrailerNumber: command.TrailerNumber is not null,
            command.DockCode,
            SetDockCode: command.DockCode is not null,
            command.DockBay,
            SetDockBay: command.DockBay is not null,
            command.DockAssignedAt,
            SetDockAssignedAt: command.DockAssignedAt is not null,
            command.AssignedToUserId,
            SetAssignedToUserId: command.AssignedToUserId is not null,
            command.WarehouseNote,
            SetWarehouseNote: command.WarehouseNote is not null,
            command.StockStatusLabel,
            SetStockStatusLabel: command.StockStatusLabel is not null,
            command.LoadingStatusLabel,
            SetLoadingStatusLabel: command.LoadingStatusLabel is not null,
            OrderDetailsMapper.ToServicesCsv(command.Services),
            SetServicesCsv: command.Services is not null,
            command.QuantityUnitLabel,
            SetQuantityUnitLabel: command.QuantityUnitLabel is not null,
            command.DockStatusLabel,
            SetDockStatusLabel: command.DockStatusLabel is not null,
            command.Status,
            command.HasAlert,
            command.AlertReason,
            SetAlertReason: command.AlertReason is not null);

        bool updated = await orderDetailsRepository.PatchOrderAsync(patch, cancellationToken);
        if (!updated)
            return Result<UpdateOrderResponse>.NotFound();

        await notificationsFeedCacheInvalidator.InvalidateAsync(cancellationToken);

        return Result.Success(new UpdateOrderResponse(command.OrderId));
    }
}
