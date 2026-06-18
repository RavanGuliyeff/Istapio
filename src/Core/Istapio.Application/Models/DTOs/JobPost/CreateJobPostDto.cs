namespace Istapio.Application.Models.DTOs.JobPost;

public record CreateJobPostDto(
    string Title,
    string Description,
    string Requirements,
    bool IsActive,
    DateTime? LastDate,
    Guid CompanyId,
    Guid CategoryId,
    Guid VacationTypeId
);