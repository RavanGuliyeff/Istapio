using FluentValidation;
using Istapio.Application.Models.DTOs.VacationType;

namespace Istapio.Application.Validators.VacationType;

public class CreateVacationTypeDtoValidator : AbstractValidator<CreateVacationTypeDto>
{
    public CreateVacationTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
    }
}