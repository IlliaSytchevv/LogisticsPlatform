namespace LogisticsPlatform.Application.Models.Dashboard;

public sealed record DashboardActivityData(
    IReadOnlyList<CompletedActivityRow> CurrentPeriodRows,
    int PreviousPeriodCompletedCount);

public sealed record CompletedActivityRow(
    DateTimeOffset CompletedAt,
    long SpendCents);
