using FluentValidation;
using Istapio.Application.Models.DTOs.VacationType;

namespace Istapio.Application.Validators.VacationType;

public class UpdateVacationTypeDtoValidator : AbstractValidator<UpdateVacationTypeDto>
{
    public UpdateVacationTypeDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
    }
}