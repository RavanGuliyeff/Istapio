namespace Istapio.Application.Models.DTOs.VacationType;

public record GetVacationTypeJobPostDto(
    Guid Id,
    string Title,
    Guid CompanyId,
    string CompanyName,
    Guid CategoryId,
    string CategoryName,
    bool IsActive,
    long ViewCount
);