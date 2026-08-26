using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.Entities;

public class OrderPayment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public long AmountCents { get; set; }
    public string Currency { get; set; } = "usd";

    public string? StripeSessionId { get; set; }
    public OrderPaymentStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
}
