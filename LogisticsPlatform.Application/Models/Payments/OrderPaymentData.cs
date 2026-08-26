using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Models.Payments;

public sealed record OrderPaymentData(
    Guid Id,
    Guid OrderId,
    long AmountCents,
    string Currency,
    string? StripeSessionId,
    OrderPaymentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt);
