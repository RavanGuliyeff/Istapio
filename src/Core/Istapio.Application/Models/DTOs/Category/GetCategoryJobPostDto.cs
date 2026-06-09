namespace Istapio.Application.Models.DTOs.Category;

public record GetCategoryJobPostDto(
    Guid Id,
    string Title,
    Guid CompanyId,
    string CompanyName,
    bool IsActive,
    long ViewCount
);
