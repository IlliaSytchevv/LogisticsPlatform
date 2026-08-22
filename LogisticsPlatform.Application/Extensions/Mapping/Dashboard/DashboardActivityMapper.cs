using LogisticsPlatform.Application.Models.Dashboard;
using LogisticsPlatform.Domain.DTO.Dashboard.Activity;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Extensions.Mapping.Dashboard;

public static class DashboardActivityMapper
{
    public static DashboardActivityResponse ToResponse(
        ActivityPeriod period,
        DashboardActivityData data,
        IReadOnlyList<ActivityBucket> buckets)
    {
        int completedTotal = data.CurrentPeriodRows.Count;
        long spendTotal = data.CurrentPeriodRows.Sum(x => x.SpendCents);

        var completedSeries = new List<ActivitySeriesPointResponse>(buckets.Count);
        var spendSeries = new List<ActivitySeriesPointResponse>(buckets.Count);

        foreach (ActivityBucket bucket in buckets)
        {
            int completed = 0;
            long spend = 0;

            foreach (CompletedActivityRow row in data.CurrentPeriodRows)
            {
                if (row.CompletedAt < bucket.Start || row.CompletedAt >= bucket.End)
                    continue;

                completed++;
                spend += row.SpendCents;
            }

            completedSeries.Add(new ActivitySeriesPointResponse(bucket.Label, completed, 0));
            spendSeries.Add(new ActivitySeriesPointResponse(bucket.Label, 0, spend));
        }

        int previousCompleted = data.PreviousPeriodCompletedCount;
        int growthPercent = previousCompleted == 0
            ? completedTotal > 0 ? 100 : 0
            : (int)Math.Round((completedTotal - previousCompleted) * 100d / previousCompleted);

        ActivitySeriesPointResponse? bestSpend = spendSeries
            .Where(x => x.ValueCents > 0)
            .OrderByDescending(x => x.ValueCents)
            .FirstOrDefault();

        long averageSpend = completedTotal == 0 ? 0 : spendTotal / completedTotal;

        return new DashboardActivityResponse(
            period,
            completedTotal,
            spendTotal,
            completedSeries,
            spendSeries,
            new DashboardActivityInsightsResponse(
                growthPercent,
                spendTotal,
                averageSpend,
                bestSpend?.Label,
                bestSpend?.ValueCents ?? 0));
    }

    public static (
        DateTimeOffset RangeStart,
        DateTimeOffset PreviousStart,
        IReadOnlyList<ActivityBucket> Buckets)
        CreateBuckets(ActivityPeriod period, DateTimeOffset now) =>
        period switch
        {
            ActivityPeriod.Day => CreateHourlyBuckets(now),
            ActivityPeriod.CW => CreateDailyBuckets(StartOfWeek(now), 7, "D"),
            ActivityPeriod.Month => CreateWeeklyBuckets(now, 10),
            ActivityPeriod.Quarter => CreateMonthlyBuckets(now, 3),
            _ => CreateWeeklyBuckets(now, 10)
        };

    private static (DateTimeOffset, DateTimeOffset, IReadOnlyList<ActivityBucket>) CreateHourlyBuckets(
        DateTimeOffset now)
    {
        DateTimeOffset rangeEnd = StartOfHour(now).AddHours(1);
        DateTimeOffset rangeStart = rangeEnd.AddHours(-24);

        ActivityBucket[] buckets = Enumerable
            .Range(0, 24)
            .Select(i =>
            {
                DateTimeOffset start = rangeStart.AddHours(i);
                return new ActivityBucket($"H{i + 1}", start, start.AddHours(1));
            })
            .ToArray();

        return (rangeStart, rangeStart.AddHours(-24), buckets);
    }

    private static (DateTimeOffset, DateTimeOffset, IReadOnlyList<ActivityBucket>) CreateDailyBuckets(
        DateTimeOffset start,
        int count,
        string labelPrefix)
    {
        ActivityBucket[] buckets = Enumerable
            .Range(0, count)
            .Select(i =>
            {
                DateTimeOffset bucketStart = start.AddDays(i);
                return new ActivityBucket($"{labelPrefix}{i + 1}", bucketStart, bucketStart.AddDays(1));
            })
            .ToArray();

        return (start, start.AddDays(-count), buckets);
    }

    private static (DateTimeOffset, DateTimeOffset, IReadOnlyList<ActivityBucket>) CreateWeeklyBuckets(
        DateTimeOffset now,
        int count)
    {
        DateTimeOffset rangeStart = StartOfWeek(now).AddDays(-7 * (count - 1));

        ActivityBucket[] buckets = Enumerable
            .Range(0, count)
            .Select(i =>
            {
                DateTimeOffset start = rangeStart.AddDays(i * 7);
                return new ActivityBucket($"W{i + 1}", start, start.AddDays(7));
            })
            .ToArray();

        return (rangeStart, rangeStart.AddDays(-7 * count), buckets);
    }

    private static (DateTimeOffset, DateTimeOffset, IReadOnlyList<ActivityBucket>) CreateMonthlyBuckets(
        DateTimeOffset now,
        int count)
    {
        DateTimeOffset currentMonth = new(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset rangeStart = currentMonth.AddMonths(-(count - 1));

        ActivityBucket[] buckets = Enumerable
            .Range(0, count)
            .Select(i =>
            {
                DateTimeOffset start = rangeStart.AddMonths(i);
                return new ActivityBucket($"M{i + 1}", start, start.AddMonths(1));
            })
            .ToArray();

        return (rangeStart, rangeStart.AddMonths(-count), buckets);
    }

    private static DateTimeOffset StartOfWeek(DateTimeOffset value)
    {
        int daysFromMonday = (7 + (value.DayOfWeek - DayOfWeek.Monday)) % 7;
        return new DateTimeOffset(value.UtcDateTime.Date.AddDays(-daysFromMonday), TimeSpan.Zero);
    }

    private static DateTimeOffset StartOfHour(DateTimeOffset value) =>
        new(value.Year, value.Month, value.Day, value.Hour, 0, 0, TimeSpan.Zero);
}

public sealed record ActivityBucket(
    string Label,
    DateTimeOffset Start,
    DateTimeOffset End);
