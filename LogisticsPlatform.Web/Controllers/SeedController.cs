using LogisticsPlatform.Infrastructure.Database;
using LogisticsPlatform.Infrastructure.Database.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Controllers;

[ApiController]
//[Authorize(Roles = "Admin")]
[Route(ApiRoutes.ApiRoutes.Seed)]
public sealed class SeedController(IServiceProvider serviceProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        await SeedData.InitializeAsync(serviceProvider);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return Ok(new
        {
            Message = "Seed completed",
            Users = await db.Users.CountAsync(),
            Hubs = await db.Hubs.CountAsync(),
            HubDocks = await db.HubDocks.CountAsync(),
            Carriers = await db.Carriers.CountAsync(),
            CatalogSkus = await db.SupplyCatalogItems.CountAsync(),
            Orders = await db.Orders.CountAsync(),
            Operations = await db.OrderOperations.CountAsync(),
            Supplies = await db.OrderSupplies.CountAsync()
        });
    }
}