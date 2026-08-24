using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    OrderType Type,
    Guid HubId,
    Guid CreatedByUserId,
    DateTimeOffset? ScheduledAt,
    string? DestinationCity,
    string? DestinationRegion,
    string? PrimaryReference,
    IReadOnlyList<CreateOrderSupplyLineRequest>? Supplies) : ICommand<Result<CreateOrderResponse>>;
