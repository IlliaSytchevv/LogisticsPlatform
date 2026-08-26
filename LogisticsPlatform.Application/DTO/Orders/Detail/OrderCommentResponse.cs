namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderCommentResponse(
    Guid Id,
    string Text,
    string? AuthorName,
    DateTimeOffset CreatedAt);
