using FluentValidation;
using Istapio.Application.Models.DTOs.Company;
using System.Text.RegularExpressions;

namespace Istapio.Application.Validators.Company;

public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name cannot be empty or whitespace");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Description cannot be empty or whitespace");

        //RuleFor(x => x.LogoUrl)
        //    .MaximumLength(500).WithMessage("LogoUrl must not exceed 500 characters")
        //    .Matches(@"^(https?://)?[\w\-._~:/?#[\]@!$&'()*+,;=.]+$", RegexOptions.IgnoreCase)
        //    .When(x => !string.IsNullOrEmpty(x.LogoUrl))
        //    .WithMessage("LogoUrl must be a valid URL");

        //RuleFor(x => x.UserId)
        //    .NotEmpty().WithMessage("UserId is required");
    }
}