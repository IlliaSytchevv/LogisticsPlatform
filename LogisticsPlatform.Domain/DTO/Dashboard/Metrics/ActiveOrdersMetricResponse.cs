namespace LogisticsPlatform.Domain.DTO.Dashboard.Metrics;

public sealed record ActiveOrdersMetricResponse(
    int Count,
    int DeltaThisWeek);
