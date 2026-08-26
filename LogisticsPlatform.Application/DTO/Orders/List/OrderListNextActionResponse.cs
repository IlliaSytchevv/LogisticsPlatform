using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Orders.List;

public sealed record OrderListNextActionResponse(
    string Label,
    NextActionKind? Kind,
    int? DueInSeconds,
    bool IsAlert,
    long? AmountCents,
    string? DocumentNumber);
