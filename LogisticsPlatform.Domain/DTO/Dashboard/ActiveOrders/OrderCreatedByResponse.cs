using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Dashboard.ActiveOrders;

public sealed record OrderCreatedByResponse(
    string Name,
    string Initials,
    UserRole Role);
