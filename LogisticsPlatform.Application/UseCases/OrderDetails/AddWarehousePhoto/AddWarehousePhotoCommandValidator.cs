using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed class AddWarehousePhotoCommandValidator : AbstractValidator<AddWarehousePhotoCommand>
{
    public const int MaxPhotosPerOrder = 5;
    public const long MaxFileBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public AddWarehousePhotoCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Only jpeg, png, webp and gif images are allowed.");
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(c => c.Length <= MaxFileBytes)
            .WithMessage($"File size must be <= {MaxFileBytes / (1024 * 1024)} MB.");
    }
}
