using Microsoft.AspNetCore.Http;

namespace Istapio.Application.Models.DTOs.Company;

public record CreateCompanyDto(
    string Name,
    string Description,
    IFormFile Logo
);