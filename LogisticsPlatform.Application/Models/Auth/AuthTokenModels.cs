namespace LogisticsPlatform.Application.Models.Auth;

public sealed record LoginTokensData(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshExpiresUtc);

public sealed record RefreshTokensData(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshExpiresUtc);
