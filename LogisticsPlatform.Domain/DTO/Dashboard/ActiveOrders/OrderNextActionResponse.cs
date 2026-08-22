using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;

public sealed record OrderNextActionResponse(
    string Label,
    NextActionKind? Kind,
    int? DueInSeconds,
    bool IsAlert,
    long? AmountCents,
    string? DocumentNumber);
