namespace Istapio.Application.Models.DTOs.Company;

public record GetCompanyJobPostDto(
    Guid Id,
    string Title,
    Guid CategoryId,
    string CategoryName,
    Guid VacationTypeId,
    string VacationTypeName,
    bool IsActive,
    long ViewCount
);