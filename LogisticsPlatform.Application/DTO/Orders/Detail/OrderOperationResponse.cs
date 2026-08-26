using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderOperationResponse(
    Guid Id,
    OrderOperationType Type,
    string TypeLabel,
    string? Trailer,
    int Quantity,
    PalletUnit Unit,
    string? UnitLabel,
    DateTimeOffset AppliedAt,
    int CommentCount,
    int PhotoCount);
