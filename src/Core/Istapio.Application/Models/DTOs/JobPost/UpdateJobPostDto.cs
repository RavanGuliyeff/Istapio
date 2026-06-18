using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.JobPost;

public record UpdateJobPostDto(
    Guid Id,
    string Title,
    string Description,
    string Requirements,
    bool IsActive,
    DateTime? LastDate,
    Guid CategoryId,
    Guid VacationTypeId
) : BaseDto(Id);