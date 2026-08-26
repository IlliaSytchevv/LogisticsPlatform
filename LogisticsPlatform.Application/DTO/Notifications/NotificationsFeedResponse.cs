namespace LogisticsPlatform.Application.DTO.Notifications;

public sealed record NotificationsFeedResponse(
    IReadOnlyList<NotificationFeedItemResponse> Items);
