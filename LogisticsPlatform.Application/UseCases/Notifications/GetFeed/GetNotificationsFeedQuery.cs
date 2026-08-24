using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Notifications;

namespace LogisticsPlatform.Application.UseCases.Notifications.GetFeed;

public sealed record GetNotificationsFeedQuery(
    int Days,
    int Take) : IQuery<Result<NotificationsFeedResponse>>;
