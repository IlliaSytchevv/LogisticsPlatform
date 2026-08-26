namespace LogisticsPlatform.Application.DTO.Supplies;

public sealed record AddSupplyFromCatalogRequest(
    Guid CatalogItemId,
    int Quantity);
