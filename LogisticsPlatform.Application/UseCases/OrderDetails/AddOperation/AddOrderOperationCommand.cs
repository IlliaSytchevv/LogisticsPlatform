using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;

public sealed record AddOrderOperationCommand(
    Guid OrderId,
    OrderOperationType Type,
    string? Trailer,
    int Quantity,
    PalletUnit Unit,
    string? UnitLabel,
    DateTimeOffset? AppliedAt) : ICommand<Result<OrderOperationResponse>>;
