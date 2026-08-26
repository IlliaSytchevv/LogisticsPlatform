namespace LogisticsPlatform.Application.DTO.Dashboard.Metrics;

public sealed record CompletedOrdersMetricResponse(
    int Count,
    int VsPreviousMonth);
