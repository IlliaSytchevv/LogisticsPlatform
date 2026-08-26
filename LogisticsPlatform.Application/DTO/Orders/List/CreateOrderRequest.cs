using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Orders.List;

public sealed record CreateOrderRequest(
    OrderType Type,
    Guid HubId,
    DateTimeOffset? ScheduledAt,
    string? DestinationCity,
    string? DestinationRegion,
    string? PrimaryReference,
    IReadOnlyList<CreateOrderSupplyLineRequest>? Supplies = null);
