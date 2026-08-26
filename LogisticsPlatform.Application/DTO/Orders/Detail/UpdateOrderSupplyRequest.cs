namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record UpdateOrderSupplyRequest(
    string Sku,
    string Name,
    string Category,
    int Quantity,
    long UnitPriceCents);
