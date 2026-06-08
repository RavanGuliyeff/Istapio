using FluentValidation;
using Istapio.Application.Models.DTOs.JobPost;

namespace Istapio.Application.Validators.JobPost;

public class CreateJobPostDtoValidator : AbstractValidator<CreateJobPostDto>
{
    public CreateJobPostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Title cannot be empty or whitespace");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Description cannot be empty or whitespace");

        RuleFor(x => x.Requirements)
            .NotEmpty().WithMessage("Requirements is required")
            .MaximumLength(5000).WithMessage("Requirements must not exceed 5000 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Requirements cannot be empty or whitespace");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required");

        RuleFor(x => x.LastDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.LastDate.HasValue)
            .WithMessage("LastDate must be in the future");
    }
}