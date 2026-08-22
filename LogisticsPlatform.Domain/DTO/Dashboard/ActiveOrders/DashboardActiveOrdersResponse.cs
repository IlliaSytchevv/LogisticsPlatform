namespace LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;

public sealed record DashboardActiveOrdersResponse(
    IReadOnlyList<OrderCardResponse> Items);
