using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrdersRepository
{
    Task<OrdersListData> GetOrdersAsync(
        OrdersListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<OrdersTabCountsData> GetTabCountsAsync(
        OrdersListFilter filter,
        CancellationToken cancellationToken);

    Task<OrdersFilterOptionsData> GetFilterOptionsAsync(CancellationToken cancellationToken);

    Task<bool> HubExistsAsync(Guid hubId, CancellationToken cancellationToken);

    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);

    Task<OrderCreatedData> CreateDraftAsync(
        OrderType type,
        Guid hubId,
        Guid createdByUserId,
        DateTimeOffset scheduledAt,
        string destinationCity,
        string destinationRegion,
        string? primaryReference,
        CancellationToken cancellationToken);
}