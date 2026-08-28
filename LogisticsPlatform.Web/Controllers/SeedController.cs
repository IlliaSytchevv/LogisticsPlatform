using LogisticsPlatform.Infrastructure.Database.Seed;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[ApiController]
[Route(ApiRoutes.ApiRoutes.Seed)]
public sealed class SeedController(IServiceProvider serviceProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        await SeedData.InitializeAsync(serviceProvider);
        
        return Ok(new
        {
            Message = "Seed completed"
        });
    }
}