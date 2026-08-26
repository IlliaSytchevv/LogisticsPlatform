using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Authorization;

namespace LogisticsPlatform.Application.UseCases.Auth.Register;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    string Role) : ICommand<Result<RegisterResponse>>;
