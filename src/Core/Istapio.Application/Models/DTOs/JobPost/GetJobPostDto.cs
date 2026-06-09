using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.JobPost;

public record GetJobPostDto(
    Guid Id,
    string Title,
    bool IsActive,
    long ViewCount,
    DateTime? LastDate,

    Guid CompanyId,
    string CompanyName,

    Guid CategoryId,
    string CategoryName

) : BaseDto(Id);
