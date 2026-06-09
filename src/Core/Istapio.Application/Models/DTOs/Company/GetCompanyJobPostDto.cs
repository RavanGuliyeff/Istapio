namespace Istapio.Application.Models.DTOs.Company;

public record GetCompanyJobPostDto(
    Guid Id,
    string Title,
    Guid CategoryId,
    string CategoryName,
    bool IsActive,
    long ViewCount
);