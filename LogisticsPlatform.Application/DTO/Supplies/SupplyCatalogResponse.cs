namespace LogisticsPlatform.Application.DTO.Supplies;

public sealed record SupplyCatalogResponse(
    IReadOnlyList<SupplyCatalogItemResponse> Items);
