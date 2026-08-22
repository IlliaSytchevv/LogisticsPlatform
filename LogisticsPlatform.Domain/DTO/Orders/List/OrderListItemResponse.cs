using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.List;

public sealed record OrderListItemResponse(
    Guid Id,
    string Number,
    OrderType Type,
    OrderStatus Status,
    string TypeLabel,
    string StatusLabel,
    string Subtitle,
    string ReferenceSummary,
    bool HasAlert,
    string? AlertReason,
    bool IsDraftIncomplete,
    OrderListCreatedByResponse CreatedBy,
    IReadOnlyList<OrderListReferenceResponse> References,
    string Hub,
    DateTimeOffset ScheduledAt,
    string QuantityDisplay,
    int? DeclaredQty,
    int? ActualQty,
    string CarrierDisplay,
    OrderListNextActionResponse NextAction);
