using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.Notifications.GetFeed;

public sealed class GetNotificationsFeedQueryValidator
    : AbstractValidator<GetNotificationsFeedQuery>
{
    public GetNotificationsFeedQueryValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(1, 90);
        
        RuleFor(x => x.Take)
            .InclusiveBetween(1, 50);
    }
}
