namespace LogisticsPlatform.Domain.DTO.Orders.List;

public sealed record CreateOrderSupplyLineRequest(
    Guid CatalogItemId,
    int Quantity);
