using FluentValidation;
using Istapio.Application.Models.DTOs.JobPost;

namespace Istapio.Application.Validators.JobPost;

public class UpdateJobPostDtoValidator : AbstractValidator<UpdateJobPostDto>
{
    public UpdateJobPostDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.Requirements)
            .NotEmpty().WithMessage("Requirements is required")
            .MaximumLength(5000).WithMessage("Requirements must not exceed 5000 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required");

        RuleFor(x => x.LastDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.LastDate.HasValue)
            .WithMessage("LastDate must be in the future");
    }
}