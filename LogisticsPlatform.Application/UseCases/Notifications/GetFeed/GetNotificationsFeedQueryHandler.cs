using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Notifications;
using LogisticsPlatform.Application.Extensions.Mapping.Notifications;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Notifications;

namespace LogisticsPlatform.Application.UseCases.Notifications.GetFeed;

public sealed class GetNotificationsFeedQueryHandler(INotificationsRepository notificationsRepository)
    : IQueryHandler<GetNotificationsFeedQuery, Result<NotificationsFeedResponse>>
{
    public async Task<Result<NotificationsFeedResponse>> Handle(
        GetNotificationsFeedQuery query,
        CancellationToken cancellationToken)
    {
        NotificationsFeedData data = await notificationsRepository.GetFeedAsync(
            query.Days,
            query.Take,
            cancellationToken);

        return Result.Success(NotificationsFeedMapper.ToResponse(data));
    }
}
