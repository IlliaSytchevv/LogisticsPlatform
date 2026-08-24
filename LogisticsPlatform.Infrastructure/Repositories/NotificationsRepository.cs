using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Notifications;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories;

public sealed class NotificationsRepository(AppDbContext dbContext) : INotificationsRepository
{
    public async Task<NotificationsFeedData> GetFeedAsync(
        int days,
        int take,
        CancellationToken cancellationToken)
    {
        DateTimeOffset since = DateTimeOffset.UtcNow.AddDays(-days);

        var rows = await dbContext.Orders
            .AsNoTracking()
            .Where(o =>
                o.CreatedAt >= since &&
                (o.HasAlert || o.NextAction.AwaitingClientAction))
            .OrderByDescending(o => o.CreatedAt)
            .Take(take)
            .Select(o => new
            {
                o.Id,
                o.Number,
                o.HasAlert,
                o.AlertReason,
                o.NextAction.AwaitingClientAction,
                o.NextAction.NextActionLabel,
                o.CreatedAt,
                o.Status
            })
            .ToListAsync(cancellationToken);

        IReadOnlyList<NotificationFeedItemData> items = rows
            .Select(o =>
            {
                bool isAlert = o.HasAlert || o.Status == OrderStatus.Alert;
                string kind = isAlert ? "alert" : "awaiting";
                string title = isAlert
                    ? $"Alert: {o.AlertReason ?? "needs attention"}"
                    : $"Awaiting: {o.NextActionLabel ?? "your action"}";

                return new NotificationFeedItemData(
                    o.Id,
                    o.Number,
                    kind,
                    title,
                    o.CreatedAt);
            })
            .ToList();

        return new NotificationsFeedData(items);
    }
}