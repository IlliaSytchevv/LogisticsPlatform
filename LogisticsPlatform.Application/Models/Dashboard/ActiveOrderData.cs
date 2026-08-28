using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Models.Dashboard;

public sealed record ActiveOrderData(
    Guid Id,
    string Number,
    OrderType Type,
    OrderStatus Status,
    bool HasAlert,
    string CreatedByName,
    string CreatedByInitials,
    UserRole CreatedByRole,
    string Hub,
    DateTimeOffset ScheduledAt,
    int? DeclaredQty,
    int? ActualQty,
    string? CarrierName,
    string DestinationCity,
    string DestinationRegion,
    string? DestinationNote,
    int? TrailersConsolidated,
    string? NextActionLabel,
    NextActionKind? NextActionKind,
    DateTimeOffset? NextActionDueAt,
    long? NextActionAmountCents,
    string? NextActionDocumentNumber,
    IReadOnlyList<ActiveOrderQuantityLineData> QuantityLines,
    IReadOnlyList<ActiveOrderSubOrderData> SubOrders);

public sealed record ActiveOrderQuantityLineData(PalletUnit Unit, int Count);

public sealed record ActiveOrderSubOrderData(
    string Number,
    string Reference,
    int PalletCount,
    bool HasMissingPhoto);
