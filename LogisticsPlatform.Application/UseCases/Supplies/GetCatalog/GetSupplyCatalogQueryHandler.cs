using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Supplies;
using LogisticsPlatform.Application.Extensions.Mapping.Supplies;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Supplies;

namespace LogisticsPlatform.Application.UseCases.Supplies.GetCatalog;

public sealed class GetSupplyCatalogQueryHandler(ISupplyCatalogRepository supplyCatalogRepository)
    : IQueryHandler<GetSupplyCatalogQuery, Result<SupplyCatalogResponse>>
{
    public async Task<Result<SupplyCatalogResponse>> Handle(
        GetSupplyCatalogQuery query,
        CancellationToken cancellationToken)
    {
        SupplyCatalogData data = await supplyCatalogRepository.GetActiveCatalogAsync(cancellationToken);
        
        return Result.Success(SupplyCatalogMapper.ToResponse(data));
    }
}
