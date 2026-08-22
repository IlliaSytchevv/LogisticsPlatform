using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Dashboard.Activity;

public sealed record DashboardActivityResponse(
    ActivityPeriod Period,
    int CompletedTotal,
    long SpendCentsTotal,
    IReadOnlyList<ActivitySeriesPointResponse> CompletedSeries,
    IReadOnlyList<ActivitySeriesPointResponse> SpendSeries,
    DashboardActivityInsightsResponse Insights);
