namespace LogisticsPlatform.Domain.DTO.Supplies;

public sealed record SupplyCatalogResponse(
    IReadOnlyList<SupplyCatalogItemResponse> Items);
