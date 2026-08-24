using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Supplies;

namespace LogisticsPlatform.Application.UseCases.Supplies.GetCatalog;

public sealed record GetSupplyCatalogQuery : IQuery<Result<SupplyCatalogResponse>>;
