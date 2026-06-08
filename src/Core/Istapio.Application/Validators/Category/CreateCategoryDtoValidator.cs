using FluentValidation;
using Istapio.Application.Models.DTOs.Category;

namespace Istapio.Application.Validators.Category;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name cannot be empty or whitespace");

        RuleFor(x => x.ParentId)
            .NotEqual(Guid.Empty).When(x => x.ParentId.HasValue)
            .WithMessage("ParentId must be a valid non-empty GUID");
    }
}