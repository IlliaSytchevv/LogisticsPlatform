namespace LogisticsPlatform.Domain.Entities;

public class OrderComment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Text { get; set; } = null!;
    public string? AuthorName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
