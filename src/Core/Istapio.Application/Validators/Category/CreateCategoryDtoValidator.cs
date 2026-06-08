using FluentValidation;
using Istapio.Application.Models.DTOs.Category;

namespace Istapio.Application.Validators.Category;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.ParentId)
            .Empty().Unless(x => x.ParentId.HasValue)
            .WithMessage("ParentId must be a valid GUID if provided");
    }
}