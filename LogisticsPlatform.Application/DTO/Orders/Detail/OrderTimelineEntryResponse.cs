namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderTimelineEntryResponse(
    Guid Id,
    string Kind,
    string Text,
    string? AuthorName,
    DateTimeOffset CreatedAt);
