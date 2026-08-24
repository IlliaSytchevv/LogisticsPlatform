using LogisticsPlatform.Application.Models.Notifications;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface INotificationsRepository
{
    Task<NotificationsFeedData> GetFeedAsync(
        int days,
        int take,
        CancellationToken cancellationToken);
}
