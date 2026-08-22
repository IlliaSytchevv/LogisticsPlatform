using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.Export;

public sealed record OrdersExportRequest(
    OrderListTab Tab = OrderListTab.All,
    Guid? HubId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    OrderStatus? Status = null,
    string? Q = null);
