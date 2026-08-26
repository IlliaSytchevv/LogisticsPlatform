using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderAccessRepository(AppDbContext dbContext) : IOrderAccessRepository
{
    public async Task<bool> ExistsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .AnyAsync(o => o.Id == orderId, cancellationToken);
    }
}
