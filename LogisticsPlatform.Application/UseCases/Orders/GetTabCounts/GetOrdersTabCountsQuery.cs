using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.TabCounts;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.GetTabCounts;

public sealed record GetOrdersTabCountsQuery(
    Guid? HubId,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    OrderStatus? Status,
    string? Q) : IQuery<Result<OrdersTabCountsResponse>>;
