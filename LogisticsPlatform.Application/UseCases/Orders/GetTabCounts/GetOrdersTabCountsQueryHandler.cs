using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.TabCounts;
using LogisticsPlatform.Application.Extensions.Mapping.Orders;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.Orders.GetTabCounts;

public sealed class GetOrdersTabCountsQueryHandler(IOrdersRepository ordersRepository)
    : IQueryHandler<GetOrdersTabCountsQuery, Result<OrdersTabCountsResponse>>
{
    public async Task<Result<OrdersTabCountsResponse>> Handle(
        GetOrdersTabCountsQuery query,
        CancellationToken cancellationToken)
    {
        var filter = new OrdersListFilter(
            Tab: null,
            query.HubId,
            query.DateFrom,
            query.DateTo,
            query.Status,
            query.Search);

        OrdersTabCountsData data = await ordersRepository.GetTabCountsAsync(filter, cancellationToken);
        
        return Result.Success(OrdersTabCountsMapper.ToResponse(data));
    }
}
