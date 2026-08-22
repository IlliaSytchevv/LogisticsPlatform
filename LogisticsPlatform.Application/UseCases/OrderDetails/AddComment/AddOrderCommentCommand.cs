using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddComment;

public sealed record AddOrderCommentCommand(
    Guid OrderId,
    string Text,
    string? AuthorName) : ICommand<Result<OrderCommentResponse>>;
