namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderSupplyResponse(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    int Quantity,
    long UnitPriceCents,
    long LineTotalCents);
