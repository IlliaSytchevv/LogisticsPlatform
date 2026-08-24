using LogisticsPlatform.Application.Models.Notifications;
using LogisticsPlatform.Domain.DTO.Notifications;

namespace LogisticsPlatform.Application.Extensions.Mapping.Notifications;

public static class NotificationsFeedMapper
{
    public static NotificationsFeedResponse ToResponse(NotificationsFeedData data) =>
        new(data.Items.Select(ToResponse).ToList());

    private static NotificationFeedItemResponse ToResponse(NotificationFeedItemData item) =>
        new(item.OrderId, item.OrderNumber, item.Kind, item.Title, item.CreatedAt);
}
