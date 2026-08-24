using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Supplies;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories;

public sealed class SupplyCatalogRepository(AppDbContext dbContext) : ISupplyCatalogRepository
{
    public async Task<SupplyCatalogData> GetActiveCatalogAsync(CancellationToken cancellationToken)
    {
        List<SupplyCatalogItemData> items = await dbContext.SupplyCatalogItems
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Sku)
            .Select(x => new SupplyCatalogItemData(
                x.Id,
                x.Sku,
                x.Name,
                x.Category,
                x.PlatformPriceCents))
            .ToListAsync(cancellationToken);

        return new SupplyCatalogData(items);
    }

    public async Task<SupplyCatalogItemInternalData?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await dbContext.SupplyCatalogItems
            .AsNoTracking()
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new SupplyCatalogItemInternalData(
                x.Id,
                x.Sku,
                x.Name,
                x.Category,
                x.PlatformPriceCents,
                x.WholesalePriceCents,
                x.MarginSplitPercent))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
