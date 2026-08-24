using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Models.Auth;

namespace LogisticsPlatform.Application.UseCases.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshTokenFromCookie)
    : ICommand<Result<RefreshTokensData>>;
