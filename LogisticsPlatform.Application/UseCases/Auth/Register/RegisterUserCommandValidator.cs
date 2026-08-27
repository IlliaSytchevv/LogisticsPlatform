using FluentValidation;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
        
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => Enum.TryParse<UserRole>(r, ignoreCase: true, out _))
            .WithMessage("Role must be Admin, Dispatcher, or Driver.");
    }
}
