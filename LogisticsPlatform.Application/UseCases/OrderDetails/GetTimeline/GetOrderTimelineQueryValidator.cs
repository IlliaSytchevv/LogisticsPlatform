using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetTimeline;

public sealed class GetOrderTimelineQueryValidator : AbstractValidator<GetOrderTimelineQuery>
{
    public GetOrderTimelineQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
    }
}
