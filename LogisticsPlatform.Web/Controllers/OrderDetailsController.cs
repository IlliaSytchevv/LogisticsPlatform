using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddComment;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationComment;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddTimelineEntry;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperation;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperationPhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteSupply;
using LogisticsPlatform.Application.UseCases.OrderDetails.DeleteWarehousePhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetBolPdf;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetComments;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationComments;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhotos;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetOrderDetails;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetQr;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetTimeline;
using LogisticsPlatform.Application.UseCases.OrderDetails.GetWarehousePhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;
using LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupply;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace LogisticsPlatform.Controllers;

[Route("api/orders")]
public sealed class OrderDetailsController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderDetails(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderDetailsQuery(orderId), cancellationToken);
        return GetActionResult(result);
    }

    [HttpPut("{orderId:guid}")]
    [HttpPatch("{orderId:guid}")]
    public async Task<IActionResult> UpdateOrder(
        Guid orderId,
        [FromBody] UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new UpdateOrderCommand(
                orderId,
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

    [HttpPost("{orderId:guid}/operations")]
    public async Task<IActionResult> AddOperation(
        Guid orderId,
        [FromBody] AddOrderOperationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderOperationCommand(
                orderId,
                request.Type,
                request.Trailer,
                request.Quantity,
                request.Unit,
                request.UnitLabel,
                request.AppliedAt),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpDelete("{orderId:guid}/operations/{operationId:guid}")]
    public async Task<IActionResult> DeleteOperation(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteOrderOperationCommand(orderId, operationId),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/operations/{operationId:guid}/comments")]
    public async Task<IActionResult> GetOperationComments(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOperationCommentsQuery(orderId, operationId),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{orderId:guid}/operations/{operationId:guid}/comments")]
    public async Task<IActionResult> AddOperationComment(
        Guid orderId,
        Guid operationId,
        [FromBody] AddOrderOperationCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOperationCommentCommand(orderId, operationId, request.Text, request.AuthorName),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/operations/{operationId:guid}/photos")]
    public async Task<IActionResult> GetOperationPhotos(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOperationPhotosQuery(orderId, operationId),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{orderId:guid}/operations/{operationId:guid}/photos")]
    [RequestSizeLimit(AddWarehousePhotoCommandValidator.MaxFileBytes)]
    public async Task<IActionResult> AddOperationPhoto(
        Guid orderId,
        Guid operationId,
        IFormFile file,
        [FromForm] int? sortOrder,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory, cancellationToken);

        var result = await Dispatcher.Send(
            new AddOperationPhotoCommand(
                orderId,
                operationId,
                file.FileName,
                file.ContentType,
                memory.ToArray(),
                sortOrder),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/operations/{operationId:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> GetOperationPhoto(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOperationPhotoQuery(orderId, operationId, photoId),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: true, cancellationToken);
    }

    [HttpDelete("{orderId:guid}/operations/{operationId:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeleteOperationPhoto(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteOperationPhotoCommand(orderId, operationId, photoId),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{orderId:guid}/supplies")]
    public async Task<IActionResult> AddSupply(
        Guid orderId,
        [FromBody] AddOrderSupplyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderSupplyCommand(
                orderId,
                request.Sku,
                request.Name,
                request.Category,
                request.Quantity,
                request.UnitPriceCents),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpPatch("{orderId:guid}/supplies/{supplyId:guid}")]
    public async Task<IActionResult> UpdateSupply(
        Guid orderId,
        Guid supplyId,
        [FromBody] UpdateOrderSupplyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new UpdateOrderSupplyCommand(
                orderId,
                supplyId,
                request.Sku,
                request.Name,
                request.Category,
                request.Quantity,
                request.UnitPriceCents),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpDelete("{orderId:guid}/supplies/{supplyId:guid}")]
    public async Task<IActionResult> DeleteSupply(
        Guid orderId,
        Guid supplyId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteOrderSupplyCommand(orderId, supplyId),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpPost("{orderId:guid}/warehouse-photos")]
    [RequestSizeLimit(AddWarehousePhotoCommandValidator.MaxFileBytes)]
    public async Task<IActionResult> AddWarehousePhoto(
        Guid orderId,
        IFormFile file,
        [FromForm] int? sortOrder,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory, cancellationToken);

        var result = await Dispatcher.Send(
            new AddWarehousePhotoCommand(
                orderId,
                file.FileName,
                file.ContentType,
                memory.ToArray(),
                sortOrder),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/warehouse-photos/{photoId:guid}")]
    public async Task<IActionResult> GetWarehousePhoto(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetWarehousePhotoQuery(orderId, photoId),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: true, cancellationToken);
    }

    [HttpDelete("{orderId:guid}/warehouse-photos/{photoId:guid}")]
    public async Task<IActionResult> DeleteWarehousePhoto(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteWarehousePhotoCommand(orderId, photoId),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderCommentsQuery(orderId), cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{orderId:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid orderId,
        [FromBody] AddOrderCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderCommentCommand(orderId, request.Text, request.AuthorName),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderTimelineQuery(orderId), cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{orderId:guid}/timeline")]
    public async Task<IActionResult> AddTimelineEntry(
        Guid orderId,
        [FromBody] AddOrderTimelineEntryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderTimelineEntryCommand(orderId, request.Text, request.AuthorName),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{orderId:guid}/bol.pdf")]
    public async Task<IActionResult> GetBolPdf(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderBolPdfQuery(orderId), cancellationToken);
        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: false, cancellationToken);
    }

    [HttpGet("{orderId:guid}/qr")]
    public async Task<IActionResult> GetQr(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderQrQuery(orderId), cancellationToken);
        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: false, cancellationToken);
    }

    private async Task<IActionResult> WriteFileAsync(
        OrderFileResponse file,
        bool inline,
        CancellationToken cancellationToken)
    {
        Response.ContentType = file.ContentType;

        var contentDisposition = new ContentDispositionHeaderValue(inline ? "inline" : "attachment");
        contentDisposition.SetHttpFileName(file.FileName);
        Response.Headers.ContentDisposition = contentDisposition.ToString();

        await file.WriteToAsync(Response.Body, cancellationToken);
        return new EmptyResult();
    }
}
