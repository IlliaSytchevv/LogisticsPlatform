namespace LogisticsPlatform.Application.Interfaces.Services;

public interface INotificationsFeedCacheInvalidator
{
    Task InvalidateAsync(CancellationToken cancellationToken = default);
}
