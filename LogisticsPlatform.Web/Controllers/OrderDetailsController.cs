using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperation;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteSupply;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteWarehousePhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetBolPdf;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetOrderDetails;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetQr;
using LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Route("api/orders")]
public sealed class OrderDetailsController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderDetails(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderDetailsQuery(id), cancellationToken);
        return GetActionResult(result);
    }

    [HttpPut("{id:guid}")]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateOrder(
        Guid id,
        [FromBody] UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new UpdateOrderCommand(
                id,
                request.CustomerName,
                request.PrimaryReference,
                request.HubId,
                request.ScheduledAt,
                request.DeclaredQty,
                request.ActualQty,
                request.TrailerType,
                request.CarrierId,
                request.Phone,
                request.TruckNumber,
                request.TrailerNumber,
                request.DockCode,
                request.DockBay,
                request.DockAssignedAt,
                request.AssignedToUserId,
                request.WarehouseNote,
                request.StockStatusLabel,
                request.LoadingStatusLabel,
                request.Services,
                request.QuantityUnitLabel,
                request.DockStatusLabel,
                request.Status,
                request.HasAlert,
                request.AlertReason),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/operations")]
    public async Task<IActionResult> AddOperation(
        Guid id,
        [FromBody] AddOrderOperationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderOperationCommand(
                id,
                request.Type,
                request.Trailer,
                request.Quantity,
                request.Unit,
                request.UnitLabel,
                request.AppliedAt),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpDelete("{id:guid}/operations/{operationId:guid}")]
    public async Task<IActionResult> DeleteOperation(
        Guid id,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteOrderOperationCommand(id, operationId),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/supplies")]
    public async Task<IActionResult> AddSupply(
        Guid id,
        [FromBody] AddOrderSupplyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderSupplyCommand(
                id,
                request.Sku,
                request.Name,
                request.Category,
                request.Quantity,
                request.UnitPriceCents),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpDelete("{id:guid}/supplies/{supplyId:guid}")]
    public async Task<IActionResult> DeleteSupply(
        Guid id,
        Guid supplyId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteOrderSupplyCommand(id, supplyId),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/warehouse-photos")]
    public async Task<IActionResult> AddWarehousePhoto(
        Guid id,
        [FromBody] AddWarehousePhotoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddWarehousePhotoCommand(id, request.Url, request.SortOrder),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpDelete("{id:guid}/warehouse-photos/{photoId:guid}")]
    public async Task<IActionResult> DeleteWarehousePhoto(
        Guid id,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteWarehousePhotoCommand(id, photoId),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{id:guid}/bol.pdf")]
    public async Task<IActionResult> GetBolPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderBolPdfQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, cancellationToken);
    }

    [HttpGet("{id:guid}/qr")]
    public async Task<IActionResult> GetQr(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderQrQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, cancellationToken);
    }

    private async Task<IActionResult> WriteFileAsync(
        OrderFileResponse file,
        CancellationToken cancellationToken)
    {
        Response.ContentType = file.ContentType;
        Response.Headers.ContentDisposition = $"attachment; filename=\"{file.FileName}\"";
        await file.WriteToAsync(Response.Body, cancellationToken);
        return new EmptyResult();
    }
}
