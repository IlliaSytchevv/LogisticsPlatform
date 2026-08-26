namespace LogisticsPlatform.Application.DTO.Dashboard.ActiveOrders;

public sealed record DashboardActiveOrdersResponse(
    IReadOnlyList<OrderCardResponse> Items);
