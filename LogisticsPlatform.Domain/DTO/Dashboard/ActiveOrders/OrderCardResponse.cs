using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;

public sealed record OrderCardResponse(
    string Number,
    OrderType Type,
    OrderStatus Status,
    string TypeLabel,
    string StatusLabel,
    bool HasAlert,
    OrderCreatedByResponse CreatedBy,
    IReadOnlyList<OrderReferenceResponse> References,
    string Hub,
    DateTimeOffset ScheduledAt,
    string QuantityDisplay,
    int? DeclaredQty,
    int? ActualQty,
    string CarrierDisplay,
    string DestinationDisplay,
    int? TrailersConsolidated,
    OrderNextActionResponse NextAction);
