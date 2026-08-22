using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Orders.ExportOrders;
using LogisticsPlatform.Application.UseCases.Orders.GetFilterOptions;
using LogisticsPlatform.Application.UseCases.Orders.GetOrdersList;
using LogisticsPlatform.Application.UseCases.Orders.GetTabCounts;
using LogisticsPlatform.Domain.DTO.Orders.Export;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.DTO.Orders.TabCounts;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Route("api/orders")]
public sealed class OrdersController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] OrdersListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOrdersListQuery(
                request.Tab,
                request.HubId,
                request.DateFrom,
                request.DateTo,
                request.Status,
                request.Q,
                request.Page,
                request.PageSize),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("tab-counts")]
    public async Task<IActionResult> GetTabCounts(
        [FromQuery] OrdersTabCountsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetOrdersTabCountsQuery(
                request.HubId,
                request.DateFrom,
                request.DateTo,
                request.Status,
                request.Q),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetOrdersFilterOptionsQuery(), cancellationToken);
        
        return GetActionResult(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] OrdersExportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new ExportOrdersQuery(
                request.Tab,
                request.HubId,
                request.DateFrom,
                request.DateTo,
                request.Status,
                request.Q),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        OrdersExportFileResponse file = result.Value;
        Response.ContentType = file.ContentType;
        Response.Headers.ContentDisposition = $"attachment; filename=\"{file.FileName}\"";
        await file.WriteToAsync(Response.Body, cancellationToken);
        return new EmptyResult();
    }
}
