namespace LogisticsPlatform.Domain.Entities;

public class OrderTimelineEntry
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    /// <summary>Status | Manual</summary>
    public string Kind { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string? AuthorName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
