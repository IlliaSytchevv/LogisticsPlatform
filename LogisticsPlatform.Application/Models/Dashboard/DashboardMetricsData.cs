namespace LogisticsPlatform.Application.Models.Dashboard;

public sealed record DashboardMetricsData(
    int ActiveOrdersCount,
    int ActiveOrdersDeltaThisWeek,
    int CompletedLast30Days,
    int CompletedPrevious30Days,
    int NeedAttentionTotal,
    int AwaitingAction,
    int Alerts,
    IReadOnlyList<AlertSampleData> AlertSamples);

public sealed record AlertSampleData(string OrderNumber, string Reason);
