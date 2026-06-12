namespace Istapio.Application.Models.DTOs.Category;

public record GetCategoryJobPostDto(
    Guid Id,
    string Title,
    Guid CompanyId,
    string CompanyName,
    Guid VacationTypeId,
    string VacationTypeName,
    bool IsActive,
    long ViewCount
);
