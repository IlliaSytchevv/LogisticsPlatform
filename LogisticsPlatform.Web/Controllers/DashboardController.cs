using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Dashboard.GetActiveOrders;
using LogisticsPlatform.Application.UseCases.Dashboard.GetActivity;
using LogisticsPlatform.Application.UseCases.Dashboard.GetMetrics;
using LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;
using LogisticsPlatform.Domain.DTO.Dashboard.Activity;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Route("api/dashboard")]
public sealed class DashboardController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetDashboardMetricsQuery(), cancellationToken);
        
        return GetActionResult(result);
    }

    [HttpGet("active-orders")]
    public async Task<IActionResult> GetActiveOrders(
        [FromQuery] DashboardActiveOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetDashboardActiveOrdersQuery(request.Take),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity(
        [FromQuery] DashboardActivityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetDashboardActivityQuery(request.Period),
            cancellationToken);

        return GetActionResult(result);
    }
}
