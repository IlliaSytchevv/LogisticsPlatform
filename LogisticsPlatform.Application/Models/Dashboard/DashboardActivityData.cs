namespace LogisticsPlatform.Application.Models.Dashboard;

public sealed record DashboardActivityData(
    IReadOnlyList<CompletedActivityBucketAggregate> BucketAggregates,
    int PreviousPeriodCompletedCount);

public sealed record CompletedActivityBucketAggregate(
    string Label,
    int CompletedCount,
    long SpendCents);

public sealed record ActivityBucket(
    string Label,
    DateTimeOffset Start,
    DateTimeOffset End);
