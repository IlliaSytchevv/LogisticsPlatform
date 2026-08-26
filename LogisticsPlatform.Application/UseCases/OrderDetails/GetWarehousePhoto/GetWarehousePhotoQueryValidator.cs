using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetWarehousePhoto;

public sealed class GetWarehousePhotoQueryValidator : AbstractValidator<GetWarehousePhotoQuery>
{
    public GetWarehousePhotoQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.PhotoId)
            .NotEmpty();
    }
}
