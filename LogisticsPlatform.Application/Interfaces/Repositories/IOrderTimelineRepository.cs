using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderTimelineRepository
{
    Task<IReadOnlyList<OrderTimelineEntryData>> GetTimelineAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<OrderTimelineEntryData> AddTimelineEntryAsync(
        Guid orderId,
        string kind,
        string text,
        string? authorName,
        CancellationToken cancellationToken);

    Task<OrderTimelineEntryData> AddStatusChangeAsync(
        Guid orderId,
        OrderStatus? previousStatus,
        OrderStatus newStatus,
        CancellationToken cancellationToken);
}
