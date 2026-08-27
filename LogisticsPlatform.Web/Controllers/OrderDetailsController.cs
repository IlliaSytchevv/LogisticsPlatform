using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Supplies;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddComment;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationComment;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddSupplyFromCatalog;
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
using LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupplyQuantity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace LogisticsPlatform.Controllers;

[Authorize]
[Route(ApiRoutes.ApiRoutes.Order)]
public sealed class OrderDetailsController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> GetOrderDetails(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderDetailsQuery(orderId), cancellationToken);
        return GetActionResult(result);
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPatch]
    public async Task<IActionResult> UpdateOrder(
        Guid orderId,
        [FromBody] UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid userId))
        {
            return Unauthorized();
        }

        var result = await Dispatcher.Send(
            new UpdateOrderCommand(
                orderId,
                userId,
                request.Number,
                request.CustomerName,
                request.PrimaryReference,
                request.DeclaredQty,
                request.ActualQty,
                request.TrailerType,
                request.Phone,
                request.TruckNumber,
                request.TrailerNumber,
                request.DockCode,
                request.DockBay,
                request.WarehouseNote,
                request.StockStatusLabel,
                request.LoadingStatusLabel,
                request.Status,
                request.AwaitingClientAction),
            cancellationToken);

        return GetActionResult(result);
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("operations")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("operations/{operationId:guid}")]
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

    [HttpGet("operations/{operationId:guid}/comments")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("operations/{operationId:guid}/comments")]
    public async Task<IActionResult> AddOperationComment(
        Guid orderId,
        Guid operationId,
        [FromBody] AddOrderOperationCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOperationCommentCommand(orderId, operationId, request.Text, GetCurrentUserDisplayName()),
            cancellationToken);
        return GetActionResult(result);
    }

    [HttpGet("operations/{operationId:guid}/photos")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("operations/{operationId:guid}/photos")]
    [RequestSizeLimit(AddWarehousePhotoCommandValidator.MaxFileBytes)]
    public async Task<IActionResult> AddOperationPhoto(
        Guid orderId,
        Guid operationId,
        IFormFile file,
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
                memory.ToArray()),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("operations/{operationId:guid}/photos/{photoId:guid}")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("operations/{operationId:guid}/photos/{photoId:guid}")]
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

    [Authorize(Roles = "Admin")]
    [HttpPost("supplies")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("supplies/from-catalog")]
    public async Task<IActionResult> AddSupplyFromCatalog(
        Guid orderId,
        [FromBody] AddSupplyFromCatalogRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddSupplyFromCatalogCommand(orderId, request.CatalogItemId, request.Quantity),
            cancellationToken);

        return GetActionResult(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("supplies/{supplyId:guid}")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPatch("supplies/{supplyId:guid}/quantity")]
    public async Task<IActionResult> UpdateSupplyQuantity(
        Guid orderId,
        Guid supplyId,
        [FromBody] UpdateOrderSupplyQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new UpdateOrderSupplyQuantityCommand(orderId, supplyId, request.Quantity),
            cancellationToken);

        return GetActionResult(result);
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("supplies/{supplyId:guid}")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("warehouse-photos")]
    [RequestSizeLimit(AddWarehousePhotoCommandValidator.MaxFileBytes)]
    public async Task<IActionResult> AddWarehousePhoto(
        Guid orderId,
        IFormFile file,
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
                memory.ToArray()),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("warehouse-photos/{photoId:guid}")]
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

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("warehouse-photos/{photoId:guid}")]
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

    [HttpGet("comments")]
    public async Task<IActionResult> GetComments(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderCommentsQuery(orderId), cancellationToken);
        return GetActionResult(result);
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("comments")]
    public async Task<IActionResult> AddComment(
        Guid orderId,
        [FromBody] AddOrderCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderCommentCommand(orderId, request.Text, GetCurrentUserDisplayName()),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("timeline")]
    public async Task<IActionResult> GetTimeline(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderTimelineQuery(orderId), cancellationToken);
        return GetActionResult(result);
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("timeline")]
    public async Task<IActionResult> AddTimelineEntry(
        Guid orderId,
        [FromBody] AddOrderTimelineEntryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new AddOrderTimelineEntryCommand(orderId, request.Text, GetCurrentUserDisplayName()),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("bol.pdf")]
    public async Task<IActionResult> GetBolPdf(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrderBolPdfQuery(orderId), cancellationToken);
        if (!result.IsSuccess)
            return GetActionResult(result);

        return await WriteFileAsync(result.Value, inline: false, cancellationToken);
    }

    [HttpGet("qr")]
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
