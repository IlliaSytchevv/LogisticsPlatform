namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record AddOrderCommentRequest(
    string Text,
    string? AuthorName);
