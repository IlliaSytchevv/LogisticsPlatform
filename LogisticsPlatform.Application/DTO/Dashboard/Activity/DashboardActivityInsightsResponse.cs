namespace LogisticsPlatform.Application.DTO.Dashboard.Activity;

public sealed record DashboardActivityInsightsResponse(
    int CompletedGrowthPercent,
    long SpendCentsTotal,
    long AvgSpendCentsPerOrder,
    string? BestWeekLabel,
    long BestWeekPeakSpendCents);
