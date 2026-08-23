using System.ComponentModel;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.List;

public sealed record OrdersListRequest(
    OrderListTab Tab = OrderListTab.All,
    Guid? HubId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    OrderStatus? Status = null,
    [property: Description(
        "Free-text search by order number, hub name, carrier, creator, or sub-order reference/number. Examples: Markham, FR001693, TForce, REF-1103")]
    string? Search = null,
    int Page = 1,
    int PageSize = 6);
