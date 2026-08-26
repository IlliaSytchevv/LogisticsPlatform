using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Supplies.GetCatalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Authorize]
[Route(ApiRoutes.ApiRoutes.Supplies)]
public sealed class SuppliesController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog(CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new GetSupplyCatalogQuery(), cancellationToken);
        
        return GetActionResult(result);
    }
}