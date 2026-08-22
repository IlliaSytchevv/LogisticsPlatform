using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetActiveOrders;

public sealed record GetDashboardActiveOrdersQuery(int Take)
    : IQuery<Result<DashboardActiveOrdersResponse>>;
