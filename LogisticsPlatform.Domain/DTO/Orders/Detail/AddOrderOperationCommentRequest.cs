namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record AddOrderOperationCommentRequest(
    string Text,
    string? AuthorName);
