namespace LogisticsPlatform.Application.DTO.Notifications;

public sealed record NotificationsFeedRequest(
    int Days = 7,
    int Take = 20);
