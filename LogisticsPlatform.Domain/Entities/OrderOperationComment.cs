namespace LogisticsPlatform.Domain.Entities;

public class OrderOperationComment
{
    public Guid Id { get; set; }
    public Guid OperationId { get; set; }
    public OrderOperation Operation { get; set; } = null!;

    public string Text { get; set; } = null!;
    public string? AuthorName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
