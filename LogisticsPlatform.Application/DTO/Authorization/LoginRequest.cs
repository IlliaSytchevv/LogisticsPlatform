namespace LogisticsPlatform.Application.DTO.Authorization;

public sealed record LoginRequest(
    string Username,
    string Password);
