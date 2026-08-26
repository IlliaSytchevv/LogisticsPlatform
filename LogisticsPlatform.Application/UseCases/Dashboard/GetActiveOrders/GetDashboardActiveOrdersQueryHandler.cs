using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Dashboard.ActiveOrders;
using LogisticsPlatform.Application.Extensions.Mapping.Dashboard;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Dashboard;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetActiveOrders;

public sealed class GetDashboardActiveOrdersQueryHandler(IDashboardRepository dashboardRepository)
    : IQueryHandler<GetDashboardActiveOrdersQuery, Result<DashboardActiveOrdersResponse>>
{
    public async Task<Result<DashboardActiveOrdersResponse>> Handle(
        GetDashboardActiveOrdersQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ActiveOrderData> orders =
            await dashboardRepository.GetActiveOrdersAsync(query.Take, cancellationToken);

        return Result.Success(DashboardActiveOrdersMapper.ToResponse(orders, DateTimeOffset.UtcNow));
    }
}
