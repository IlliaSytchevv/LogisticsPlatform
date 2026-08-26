namespace LogisticsPlatform.Application.DTO.Payments;

public sealed record CreateCheckoutResponse(
    Guid PaymentId,
    string CheckoutUrl,
    long AmountCents,
    string Currency);
