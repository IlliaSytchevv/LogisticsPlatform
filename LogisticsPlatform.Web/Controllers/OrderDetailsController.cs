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

    [HttpGet("{id:guid}/operations/{operationId:guid}/comments")]
    public async Task<IActionResult> GetOperationComments(
        Guid id,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOperationCommentsQuery(id, operationId),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/operations/{operationId:guid}/comments")]
    public async Task<IActionResult> AddOperationComment(
        Guid id,
        Guid operationId,
        [FromBody] AddOrderOperationCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOperationCommentCommand(id, operationId, request.Text, request.AuthorName),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpGet("{id:guid}/operations/{operationId:guid}/photos")]
    public async Task<IActionResult> GetOperationPhotos(
        Guid id,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOperationPhotosQuery(id, operationId),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/operations/{operationId:guid}/photos")]
    [RequestSizeLimit(AddWarehousePhotoCommandValidator.MaxFileBytes)]
    public async Task<IActionResult> AddOperationPhoto(
        Guid id,
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
                id,
                operationId,
                file.FileName,
                file.ContentType,
                memory.ToArray(),
                sortOrder),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{id:guid}/operations/{operationId:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> GetOperationPhoto(
        Guid id,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOperationPhotoQuery(id, operationId, photoId),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: true, cancellationToken);
    }

    [HttpDelete("{id:guid}/operations/{operationId:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeleteOperationPhoto(
        Guid id,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new DeleteOperationPhotoCommand(id, operationId, photoId),
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

    [HttpPatch("{id:guid}/supplies/{supplyId:guid}")]
    public async Task<IActionResult> UpdateSupply(
        Guid id,
        Guid supplyId,
        [FromBody] UpdateOrderSupplyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new UpdateOrderSupplyCommand(
                id,
                supplyId,
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
    [RequestSizeLimit(AddWarehousePhotoCommandValidator.MaxFileBytes)]
    public async Task<IActionResult> AddWarehousePhoto(
        Guid id,
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
                id,
                file.FileName,
                file.ContentType,
                memory.ToArray(),
                sortOrder),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{id:guid}/warehouse-photos/{photoId:guid}")]
    public async Task<IActionResult> GetWarehousePhoto(
        Guid id,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetWarehousePhotoQuery(id, photoId),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: true, cancellationToken);
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

    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderCommentsQuery(id), cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(
        Guid id,
        [FromBody] AddOrderCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderCommentCommand(id, request.Text, request.AuthorName),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderTimelineQuery(id), cancellationToken);
        return GetActionResult(result);
    }

    [HttpPost("{id:guid}/timeline")]
    public async Task<IActionResult> AddTimelineEntry(
        Guid id,
        [FromBody] AddOrderTimelineEntryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderTimelineEntryCommand(id, request.Text, request.AuthorName),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("{id:guid}/bol.pdf")]
    public async Task<IActionResult> GetBolPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderBolPdfQuery(id), cancellationToken);
        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: false, cancellationToken);
    }

    [HttpGet("{id:guid}/qr")]
    public async Task<IActionResult> GetQr(Guid id, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderQrQuery(id), cancellationToken);
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
        string disposition = inline ? "inline" : "attachment";
        Response.Headers.ContentDisposition = $"{disposition}; filename=\"{file.FileName}\"";
        await file.WriteToAsync(Response.Body, cancellationToken);
        return new EmptyResult();
    }
}
