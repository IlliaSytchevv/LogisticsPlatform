using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Dashboard.Metrics;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetMetrics;

public sealed record GetDashboardMetricsQuery : IQuery<Result<DashboardMetricsResponse>>;
