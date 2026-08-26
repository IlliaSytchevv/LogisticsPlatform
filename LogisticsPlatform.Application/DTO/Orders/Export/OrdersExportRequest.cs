using System.ComponentModel;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Orders.Export;

public sealed record OrdersExportRequest(
    OrderListTab Tab = OrderListTab.All,
    Guid? HubId = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    OrderStatus? Status = null,
    [property: Description(
        "Free-text search by order number, ref, hub, or carrier. Examples: Markham, FR001693, TForce, REF-1103")]
    string? Search = null);
