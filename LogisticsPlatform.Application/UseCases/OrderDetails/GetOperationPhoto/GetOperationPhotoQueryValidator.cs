using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhoto;

public sealed class GetOperationPhotoQueryValidator : AbstractValidator<GetOperationPhotoQuery>
{
    public GetOperationPhotoQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.OperationId)
            .NotEmpty();
        
        RuleFor(x => x.PhotoId)
            .NotEmpty();
    }
}
