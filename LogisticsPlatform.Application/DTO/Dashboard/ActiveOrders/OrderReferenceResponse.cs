namespace LogisticsPlatform.Application.DTO.Dashboard.ActiveOrders;

public sealed record OrderReferenceResponse(
    string SubOrderNumber,
    string Reference,
    string Description,
    string? Alert);
