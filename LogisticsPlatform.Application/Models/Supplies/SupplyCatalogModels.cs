namespace LogisticsPlatform.Application.Models.Supplies;

public sealed record SupplyCatalogItemData(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    long PlatformPriceCents);

public sealed record SupplyCatalogData(
    IReadOnlyList<SupplyCatalogItemData> Items);

public sealed record SupplyCatalogItemInternalData(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    long PlatformPriceCents,
    long WholesalePriceCents,
    decimal MarginSplitPercent);
