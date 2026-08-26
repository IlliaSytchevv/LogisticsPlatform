using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Dashboard.ActiveOrders;

public sealed record OrderCreatedByResponse(
    string Name,
    string Initials,
    UserRole Role);
