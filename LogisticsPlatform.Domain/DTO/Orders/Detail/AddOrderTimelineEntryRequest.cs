namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record AddOrderTimelineEntryRequest(
    string Text,
    string? AuthorName);
