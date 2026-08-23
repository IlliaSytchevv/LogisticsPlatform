using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddTimelineEntry;

public sealed record AddOrderTimelineEntryCommand(
    Guid OrderId,
    string Text,
    string? AuthorName) : ICommand<Result<OrderTimelineEntryResponse>>;
