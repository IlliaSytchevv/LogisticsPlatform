namespace LogisticsPlatform.Domain.Entities;

public class OrderOperationPhoto
{
    public Guid Id { get; set; }
    public Guid OperationId { get; set; }
    public OrderOperation Operation { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string StorageKey { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
