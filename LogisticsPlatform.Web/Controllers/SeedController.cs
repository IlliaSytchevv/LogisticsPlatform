using LogisticsPlatform.Infrastructure.Database;
using LogisticsPlatform.Infrastructure.Database.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Controllers;

[ApiController]
[Route(ApiRoutes.ApiRoutes.Seed)]
public sealed class SeedController(IServiceProvider serviceProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        await SeedData.InitializeAsync(serviceProvider);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        int foeSkuCount = await db.SupplyCatalogItems.CountAsync();

        return Ok(new
        {
            Message = "Seed completed",
            FoeCatalogSkuCount = foeSkuCount,
            Hint = foeSkuCount == 0
                ? "SupplyCatalogItems is empty — migration may not have created the table (often blocked by RefreshTokens already existing)."
                : (string?)null
        });
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
    
    [HttpPost("all-order-details")]
    public async Task<IActionResult> SeedAllOrderDetail()
    {
        var result = await SeedAllOrderDetails.InitializeAsync(serviceProvider);

        return Ok(new
        {
            Message = "All order details seed completed",
            result.HubDocksAdded,
            result.OrdersScanned,
            result.OrdersEnriched,
            result.OperationsAdded,
            result.SuppliesAdded,
            result.CommentsAdded,
            result.TimelineEntriesAdded,
            result.WarehousePhotosAdded
        });
    }
}