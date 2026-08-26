using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddTimelineEntry;

public sealed class AddOrderTimelineEntryCommandValidator : AbstractValidator<AddOrderTimelineEntryCommand>
{
    public AddOrderTimelineEntryCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
