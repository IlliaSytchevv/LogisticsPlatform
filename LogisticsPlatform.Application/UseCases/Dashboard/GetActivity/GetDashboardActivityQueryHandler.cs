using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Dashboard.Activity;
using LogisticsPlatform.Application.Extensions.Mapping.Dashboard;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Dashboard;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetActivity;

public sealed class GetDashboardActivityQueryHandler(IDashboardRepository dashboardRepository)
    : IQueryHandler<GetDashboardActivityQuery, Result<DashboardActivityResponse>>
{
    public async Task<Result<DashboardActivityResponse>> Handle(
        GetDashboardActivityQuery query,
        CancellationToken cancellationToken)
    {
        var (rangeStart, previousStart, buckets) =
            DashboardActivityMapper.CreateBuckets(query.Period, DateTimeOffset.UtcNow);

        DashboardActivityData data = await dashboardRepository.GetActivityAsync(
            rangeStart,
            previousStart,
            buckets,
            cancellationToken);

        return Result.Success(DashboardActivityMapper.ToResponse(query.Period, data));
    }
}
