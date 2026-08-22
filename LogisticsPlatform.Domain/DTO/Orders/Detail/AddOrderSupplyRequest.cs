namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record AddOrderSupplyRequest(
    string Sku,
    string Name,
    string Category,
    int Quantity,
    long UnitPriceCents);
