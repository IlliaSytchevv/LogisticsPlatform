namespace LogisticsPlatform.Domain.DTO.Supplies;

public sealed record AddSupplyFromCatalogRequest(
    Guid CatalogItemId,
    int Quantity);
