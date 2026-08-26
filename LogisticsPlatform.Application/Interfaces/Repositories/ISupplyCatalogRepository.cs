using LogisticsPlatform.Application.Models.Supplies;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface ISupplyCatalogRepository
{
    Task<SupplyCatalogData> GetActiveCatalogAsync(CancellationToken cancellationToken);

    Task<SupplyCatalogItemInternalData?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SupplyCatalogItemInternalData>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken);
}
