namespace LogisticsPlatform.Domain.DTO.Authorization;

public sealed record LoginRequest(
    string Username,
    string Password);
