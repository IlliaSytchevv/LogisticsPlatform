using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Authorization;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LogisticsPlatform.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IUserManagerWrapper userManagerWrapper,
    ILogger<RegisterUserCommandHandler> logger)
    : ICommandHandler<RegisterUserCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = command.Name,
            Email = command.Email,
            ExternalId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            DisplayName = command.Name,
            Initials = BuildInitials(command.Name),
            Role = MapDomainRole(command.Role),
            BalanceCents = 0,
            IsActive = true
        };

        var createResult = await userManagerWrapper.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors
                .Select(e => new ValidationError(e.Code, e.Description))
                .ToArray();

            return Result<RegisterResponse>.Invalid(errors);
        }

        await userManagerWrapper.AddToRoleAsync(user, command.Role);
        logger.LogInformation("User {UserId} registered with role {Role}", user.Id, command.Role);

        return Result.Success(new RegisterResponse(user.Id));
    }

    private static string BuildInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return "U";

        if (parts.Length == 1)
            return parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();

        return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
    }

    private static UserRole MapDomainRole(string identityRole) =>
        identityRole.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            ? UserRole.Admin
            : UserRole.Dispatcher;
}
