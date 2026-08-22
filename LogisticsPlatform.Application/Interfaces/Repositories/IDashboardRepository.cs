using LogisticsPlatform.Application.Models.Dashboard;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<DashboardMetricsData> GetMetricsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ActiveOrderData>> GetActiveOrdersAsync(int take, CancellationToken cancellationToken);

    Task<DashboardActivityData> GetActivityAsync(
        DateTimeOffset rangeStart,
        DateTimeOffset previousStart,
        CancellationToken cancellationToken);
}
