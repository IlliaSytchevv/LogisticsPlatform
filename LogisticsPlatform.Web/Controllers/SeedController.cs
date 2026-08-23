using LogisticsPlatform.Infrastructure.Seed;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[ApiController]
[Route("api/seed")]
public sealed class SeedController(IServiceProvider serviceProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        await SeedData.InitializeAsync(serviceProvider);

        return Ok(new { Message = "Seed completed" });
    }

    [HttpPost("demo-details")]
    public async Task<IActionResult> SeedDemoDetail()
    {
        var result = await SeedDemoDetails.InitializeAsync(serviceProvider);

        return Ok(new
        {
            Message = "Demo details seed completed",
            result.HubDocksAdded,
            result.OrdersEnriched,
            result.OrdersAdded,
            result.OperationsAdded,
            result.OperationCommentsAdded,
            result.OperationPhotosAdded,
            result.SuppliesAdded,
            result.WarehousePhotosAdded,
            result.CommentsAdded,
            result.TimelineEntriesAdded
        });
    }
}