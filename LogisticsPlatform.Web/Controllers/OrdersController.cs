using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Export;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Application.DTO.Orders.TabCounts;
using LogisticsPlatform.Application.UseCases.Orders.CreateOrder;
using LogisticsPlatform.Application.UseCases.Orders.ExportOrders;
using LogisticsPlatform.Application.UseCases.Orders.GetFilterOptions;
using LogisticsPlatform.Application.UseCases.Orders.GetOrdersList;
using LogisticsPlatform.Application.UseCases.Orders.GetTabCounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace LogisticsPlatform.Controllers;

[Authorize]
[Route(ApiRoutes.ApiRoutes.Orders)]
public sealed class OrdersController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        string? userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(userIdValue, out Guid createdByUserId))
            return Unauthorized();

        var result = await Dispatcher.Send(
            new CreateOrderCommand(
                request.Type,
                request.HubId,
                createdByUserId,
                request.ScheduledAt,
                request.DestinationCity,
                request.DestinationRegion,
                request.PrimaryReference,
                request.Supplies),
            cancellationToken);

        return GetActionResult(result);
    }

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
                request.Search,
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
                request.Search),
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
                request.Search),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        OrdersExportFileResponse file = result.Value;
        Response.ContentType = file.ContentType;

        var contentDisposition = new ContentDispositionHeaderValue("attachment");
        contentDisposition.SetHttpFileName(file.FileName);
        Response.Headers.ContentDisposition = contentDisposition.ToString();

        await file.WriteToAsync(Response.Body, cancellationToken);
        
        return new EmptyResult();
    }
}
