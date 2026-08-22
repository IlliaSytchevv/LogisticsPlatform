using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperation;

public sealed class AddOrderOperationCommandValidator : AbstractValidator<AddOrderOperationCommand>
{
    public AddOrderOperationCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Unit).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
