using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Dashboard.Metrics;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetMetrics;

public sealed record GetDashboardMetricsQuery : IQuery<Result<DashboardMetricsResponse>>;
