using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Models.Orders;

public sealed record OrderExportRowData(
    string Number,
    OrderType Type,
    OrderStatus Status,
    string Hub,
    DateTimeOffset ScheduledAt,
    string? CarrierName,
    string CreatedByName,
    UserRole CreatedByRole,
    int? DeclaredQty,
    int? ActualQty,
    string QuantityDisplay,
    string References,
    string? NextActionLabel,
    bool HasAlert,
    string? AlertReason);
