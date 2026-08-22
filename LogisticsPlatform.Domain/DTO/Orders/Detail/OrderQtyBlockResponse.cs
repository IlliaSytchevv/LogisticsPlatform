namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderQtyBlockResponse(
    int? Quantity,
    string? UnitLabel);
