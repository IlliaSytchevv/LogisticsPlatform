namespace LogisticsPlatform.Domain.DTO.Authorization;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string Role);
