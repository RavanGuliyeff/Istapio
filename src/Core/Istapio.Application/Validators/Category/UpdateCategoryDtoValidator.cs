using FluentValidation;
using Istapio.Application.Models.DTOs.Category;

namespace Istapio.Application.Validators.Category;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name cannot be empty or whitespace");

        RuleFor(x => x.ParentId)
            .NotEqual(Guid.Empty).When(x => x.ParentId.HasValue)
            .WithMessage("ParentId must be a valid non-empty GUID");

        // Cannot set itself as parent
        RuleFor(x => x.ParentId)
            .NotEqual(x => x.Id).When(x => x.ParentId.HasValue)
            .WithMessage("A category cannot be its own parent");
    }
}