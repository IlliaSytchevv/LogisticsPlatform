using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationComment;

public sealed class AddOperationCommentCommandValidator : AbstractValidator<AddOperationCommentCommand>
{
    public AddOperationCommentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.OperationId)
            .NotEmpty();
        
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
