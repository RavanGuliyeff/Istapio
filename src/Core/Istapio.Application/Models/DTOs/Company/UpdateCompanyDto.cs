using Istapio.Application.Models.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace Istapio.Application.Models.DTOs.Company;

public record UpdateCompanyDto(
    Guid Id,
    string Name,
    string Description,
    IFormFile? Logo = null
) : BaseDto(Id);