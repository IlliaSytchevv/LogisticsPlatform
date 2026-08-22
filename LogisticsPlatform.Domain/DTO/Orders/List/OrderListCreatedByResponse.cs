using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Orders.List;

public sealed record OrderListCreatedByResponse(
    string Name,
    string Initials,
    UserRole Role);
