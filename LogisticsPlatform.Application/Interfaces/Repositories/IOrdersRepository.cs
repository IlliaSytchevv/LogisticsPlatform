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
}