using FluentValidation;
using Istapio.Application.Models.DTOs.Setting;

namespace Istapio.Application.Validators.Setting;

public class UpdateSettingDtoValidator : AbstractValidator<UpdateSettingDto>
{
    public UpdateSettingDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required")
            .MaximumLength(2000).WithMessage("Value must not exceed 2000 characters");
    }
}