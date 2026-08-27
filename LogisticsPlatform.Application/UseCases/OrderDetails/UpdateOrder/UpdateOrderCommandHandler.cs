using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Domain.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed class UpdateOrderCommandHandler(
    IOrderEditLock orderEditLock,
    IOrderPatchRepository orderPatchRepository)
    : ICommandHandler<UpdateOrderCommand, Result<UpdateOrderResponse>>
{
    public async Task<Result<UpdateOrderResponse>> Handle(
        UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderEditLock.IsHeldByAsync(command.OrderId, command.UserId, cancellationToken))
        {
            return Result<UpdateOrderResponse>.Conflict("Edit lock required or held by another user.");
        }

        var current = await orderPatchRepository.GetStatusAndNumberAsync(command.OrderId, cancellationToken);

        if (current is null)
        {
            return Result<UpdateOrderResponse>.NotFound();
        }

        OrderStatus currentStatus = current.Value.Status;
        string currentNumber = current.Value.Number;

        if (command.Status is { } nextStatus)
        {
            if (!OrderStatusTransitions.IsAllowed(currentStatus, nextStatus))
            {
                return Result<UpdateOrderResponse>.Invalid(
                [
                    new ValidationError(nameof(command.Status), $"Transition from {currentStatus} to {nextStatus} is not allowed.")
                ]);
            }
        }

        OrderStatus effectiveStatus = command.Status ?? currentStatus;
        string effectiveNumber = string.IsNullOrWhiteSpace(command.Number)
            ? currentNumber
            : OrderNumberRules.Normalize(command.Number);

        if (effectiveStatus != OrderStatus.Draft && OrderNumberRules.IsDraftNumber(effectiveNumber))
        {
            return Result<UpdateOrderResponse>.Invalid(
            [
                new ValidationError(
                    nameof(command.Number),
                    "Rename the order number before leaving Draft (cannot keep a DRAFT-… number).")
            ]);
        }

        if (!string.Equals(effectiveNumber, currentNumber, StringComparison.Ordinal))
        {
            if (await orderPatchRepository.NumberExistsAsync(
                    effectiveNumber,
                    command.OrderId,
                    cancellationToken))
            {
                return Result<UpdateOrderResponse>.Conflict($"Order number '{effectiveNumber}' is already in use.");
            }
        }

        var patch = new OrderDetailPatchData(
            command.OrderId,
            string.Equals(effectiveNumber, currentNumber, StringComparison.Ordinal)
                ? null
                : effectiveNumber,
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
            command.Status,
            command.AwaitingClientAction);

        bool updated = await orderPatchRepository.PatchOrderAsync(patch, cancellationToken);
        if (!updated)
        {
            return Result<UpdateOrderResponse>.NotFound();
        }

        return Result.Success(new UpdateOrderResponse(command.OrderId));
    }
}
