namespace LogisticsPlatform.Domain.DTO.Orders.List;

public sealed record OrdersListResponse(
    int TotalCount,
    int Page,
    int PageSize,
    IReadOnlyList<OrderListItemResponse> Items);
