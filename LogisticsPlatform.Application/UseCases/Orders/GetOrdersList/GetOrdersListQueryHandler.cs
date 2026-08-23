using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.Orders;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.List;

namespace LogisticsPlatform.Application.UseCases.Orders.GetOrdersList;

public sealed class GetOrdersListQueryHandler(IOrdersRepository ordersRepository)
    : IQueryHandler<GetOrdersListQuery, Result<OrdersListResponse>>
{
    public async Task<Result<OrdersListResponse>> Handle(
        GetOrdersListQuery query,
        CancellationToken cancellationToken)
    {
        var filter = new OrdersListFilter(
            query.Tab,
            query.HubId,
            query.DateFrom,
            query.DateTo,
            query.Status,
            query.Search);

        OrdersListData data = await ordersRepository.GetOrdersAsync(
            filter,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result.Success(OrdersListMapper.ToResponse(data, query.Page, query.PageSize, DateTimeOffset.UtcNow));
    }
}
