namespace LogisticsPlatform.Application.DTO.Supplies;

/// <summary>Client-facing catalog row — platform price only (WP / margin never included).</summary>
public sealed record SupplyCatalogItemResponse(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    long PlatformPriceCents);
