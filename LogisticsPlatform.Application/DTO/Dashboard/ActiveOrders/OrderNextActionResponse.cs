using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Dashboard.ActiveOrders;

public sealed record OrderNextActionResponse(
    string Label,
    NextActionKind? Kind,
    int? DueInSeconds,
    bool IsAlert,
    long? AmountCents,
    string? DocumentNumber);
