using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationComments;

public sealed class GetOperationCommentsQueryValidator : AbstractValidator<GetOperationCommentsQuery>
{
    public GetOperationCommentsQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.OperationId).NotEmpty();
    }
}
