using Istapio.Application.Models.DTOs.Common;
using Istapio.Application.Models.DTOs.JobPost;

namespace Istapio.Application.Models.DTOs.Company;

public record GetCompanyDto(
    Guid Id,
    string Name,
    string? LogoUrl,
    string UserId,
    string UserName
) : BaseDto(Id);
