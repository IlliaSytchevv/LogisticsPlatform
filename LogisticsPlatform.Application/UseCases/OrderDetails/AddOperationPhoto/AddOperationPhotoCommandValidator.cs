using FluentValidation;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;

public sealed class AddOperationPhotoCommandValidator : AbstractValidator<AddOperationPhotoCommand>
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public AddOperationPhotoCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.OperationId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Only jpeg, png, webp and gif images are allowed.");
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(c => c.Length <= AddWarehousePhotoCommandValidator.MaxFileBytes)
            .WithMessage($"File size must be <= {AddWarehousePhotoCommandValidator.MaxFileBytes / (1024 * 1024)} MB.");
    }
}
