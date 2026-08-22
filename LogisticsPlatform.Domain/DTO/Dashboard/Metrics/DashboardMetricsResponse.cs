namespace LogisticsPlatform.Domain.DTO.Dashboard.Metrics;

public sealed record DashboardMetricsResponse(
    ActiveOrdersMetricResponse ActiveOrders,
    CompletedOrdersMetricResponse CompletedLast30Days,
    NeedAttentionMetricResponse NeedAttention);
