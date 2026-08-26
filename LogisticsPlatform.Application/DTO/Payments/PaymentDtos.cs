namespace LogisticsPlatform.Application.DTO.Payments;

public sealed record CreateOrderCheckoutResponse(
    Guid PaymentId,
    string CheckoutUrl,
    long AmountCents,
    string Currency);

public sealed record OrderPaymentStatusResponse(
    Guid? PaymentId,
    string Status,
    long? AmountCents,
    DateTimeOffset? PaidAt);
