using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Dashboard.Metrics;
using LogisticsPlatform.Application.Extensions.Mapping.Dashboard;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Dashboard;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetMetrics;

public sealed class GetDashboardMetricsQueryHandler(IDashboardRepository dashboardRepository)
    : IQueryHandler<GetDashboardMetricsQuery, Result<DashboardMetricsResponse>>
{
    public async Task<Result<DashboardMetricsResponse>> Handle(
        GetDashboardMetricsQuery query,
        CancellationToken cancellationToken)
    {
        DashboardMetricsData data = await dashboardRepository.GetMetricsAsync(cancellationToken);
        
        return Result.Success(DashboardMetricsMapper.ToResponse(data));
    }
}