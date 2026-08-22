using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.Orders;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.FilterOptions;

namespace LogisticsPlatform.Application.UseCases.Orders.GetFilterOptions;

public sealed class GetOrdersFilterOptionsQueryHandler(IOrdersRepository ordersRepository)
    : IQueryHandler<GetOrdersFilterOptionsQuery, Result<OrdersFilterOptionsResponse>>
{
    public async Task<Result<OrdersFilterOptionsResponse>> Handle(
        GetOrdersFilterOptionsQuery query,
        CancellationToken cancellationToken)
    {
        OrdersFilterOptionsData data = await ordersRepository.GetFilterOptionsAsync(cancellationToken);
        
        return Result.Success(OrdersFilterOptionsMapper.ToResponse(data));
    }
}
