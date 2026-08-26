using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationComment;

public sealed record AddOperationCommentCommand(
    Guid OrderId,
    Guid OperationId,
    string Text,
    string? AuthorName) : ICommand<Result<OrderCommentResponse>>;
