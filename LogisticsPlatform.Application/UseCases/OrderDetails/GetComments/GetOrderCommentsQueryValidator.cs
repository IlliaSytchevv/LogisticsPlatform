using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetComments;

public sealed class GetOrderCommentsQueryValidator : AbstractValidator<GetOrderCommentsQuery>
{
    public GetOrderCommentsQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
    }
}
