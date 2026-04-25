using FluentValidation;
using Istapio.Application.Models.DTOs.Setting;

namespace Istapio.Application.Validators.Setting;

public class CreateSettingDtoValidator : AbstractValidator<CreateSettingDto>
{
    public CreateSettingDtoValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Key is required")
            .MaximumLength(100).WithMessage("Key must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9._-]+$")
            .WithMessage("Key can only contain letters, numbers, dots, underscores and hyphens");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required")
            .MaximumLength(2000).WithMessage("Value must not exceed 2000 characters");
    }
}
