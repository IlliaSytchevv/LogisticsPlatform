using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.GetOrdersList;

public sealed record GetOrdersListQuery(
    OrderListTab Tab,
    Guid? HubId,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    OrderStatus? Status,
    string? Q,
    int Page,
    int PageSize) : IQuery<Result<OrdersListResponse>>;
