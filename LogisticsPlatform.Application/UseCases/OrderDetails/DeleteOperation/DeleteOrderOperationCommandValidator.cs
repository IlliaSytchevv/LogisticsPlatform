using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperation;

public sealed class DeleteOrderOperationCommandValidator : AbstractValidator<DeleteOrderOperationCommand>
{
    public DeleteOrderOperationCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.OperationId)
            .NotEmpty();
    }
}
