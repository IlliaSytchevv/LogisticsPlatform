using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddComment;

public sealed class AddOrderCommentCommandValidator : AbstractValidator<AddOrderCommentCommand>
{
    public AddOrderCommentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.AuthorName).MaximumLength(128).When(x => x.AuthorName is not null);
    }
}
