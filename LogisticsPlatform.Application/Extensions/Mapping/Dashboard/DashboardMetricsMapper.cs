using LogisticsPlatform.Application.DTO.Dashboard.Metrics;
using LogisticsPlatform.Application.Models.Dashboard;

namespace LogisticsPlatform.Application.Extensions.Mapping.Dashboard;

public static class DashboardMetricsMapper
{
    public static DashboardMetricsResponse ToResponse(DashboardMetricsData data) =>
        new(
            new ActiveOrdersMetricResponse(data.ActiveOrdersCount, data.ActiveOrdersDeltaThisWeek),
            new CompletedOrdersMetricResponse(
                data.CompletedLast30Days,
                data.CompletedLast30Days - data.CompletedPrevious30Days),
            new NeedAttentionMetricResponse(
                data.NeedAttentionTotal,
                data.AwaitingAction,
                data.Alerts,
                data.AlertSamples
                    .Select(x => new AlertSampleResponse(x.OrderNumber, x.Reason))
                    .ToList()));
}
