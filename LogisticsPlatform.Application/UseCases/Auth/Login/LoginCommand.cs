using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Models.Auth;

namespace LogisticsPlatform.Application.UseCases.Auth.Login;

public sealed record LoginCommand(
    string Username,
    string Password) : ICommand<Result<LoginTokensData>>;
