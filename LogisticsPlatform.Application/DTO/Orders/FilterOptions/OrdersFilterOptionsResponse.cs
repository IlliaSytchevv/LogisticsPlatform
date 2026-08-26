namespace LogisticsPlatform.Application.DTO.Orders.FilterOptions;

public sealed record OrdersFilterOptionsResponse(
    IReadOnlyList<OrderHubOptionResponse> Hubs,
    IReadOnlyList<OrderStatusOptionResponse> Statuses);
