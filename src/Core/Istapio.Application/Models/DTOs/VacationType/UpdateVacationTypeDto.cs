using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.VacationType;

public record UpdateVacationTypeDto(
    Guid Id,
    string Name
) : BaseDto(Id);