using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record AddOrderOperationRequest(
    OrderOperationType Type,
    string? Trailer,
    int Quantity,
    PalletUnit Unit,
    string? UnitLabel,
    DateTimeOffset? AppliedAt);
