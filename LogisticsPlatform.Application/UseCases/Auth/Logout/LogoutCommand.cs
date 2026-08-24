using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.Auth.Logout;

public sealed record LogoutCommand(string? RefreshTokenFromCookie) : ICommand<Result>;
