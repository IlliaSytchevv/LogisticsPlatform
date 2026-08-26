namespace LogisticsPlatform.Application.DTO.Orders.List;

public sealed record OrderListReferenceResponse(
    string SubOrderNumber,
    string Reference,
    string Description,
    string? Alert);
