namespace LogisticsPlatform.Application.Models.Notifications;

public sealed record NotificationFeedItemData(
    Guid OrderId,
    string OrderNumber,
    string Kind,
    string Title,
    DateTimeOffset CreatedAt);

public sealed record NotificationsFeedData(
    IReadOnlyList<NotificationFeedItemData> Items);
