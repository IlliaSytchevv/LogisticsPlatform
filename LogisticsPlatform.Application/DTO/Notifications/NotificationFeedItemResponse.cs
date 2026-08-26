namespace LogisticsPlatform.Application.DTO.Notifications;

public sealed record NotificationFeedItemResponse(
    Guid OrderId,
    string OrderNumber,
    string Kind,
    string Title,
    DateTimeOffset CreatedAt);
