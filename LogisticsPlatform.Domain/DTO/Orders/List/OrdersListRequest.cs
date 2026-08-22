using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.List;

public sealed record OrdersListRequest(
    OrderListTab Tab = OrderListTab.All,
    Guid? HubId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    OrderStatus? Status = null,
    string? Q = null,
    int Page = 1,
    int PageSize = 6);
