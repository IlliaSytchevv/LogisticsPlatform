using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.TabCounts;

public sealed record OrdersTabCountsRequest(
    Guid? HubId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    OrderStatus? Status = null,
    string? Q = null);
