namespace LogisticsPlatform.Domain.DTO.Notifications;

public sealed record NotificationsFeedResponse(
    IReadOnlyList<NotificationFeedItemResponse> Items);
