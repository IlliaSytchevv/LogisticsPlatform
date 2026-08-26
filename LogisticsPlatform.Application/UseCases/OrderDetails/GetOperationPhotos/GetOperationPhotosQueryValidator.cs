using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhotos;

public sealed class GetOperationPhotosQueryValidator : AbstractValidator<GetOperationPhotosQuery>
{
    public GetOperationPhotosQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.OperationId)
            .NotEmpty();
    }
}
