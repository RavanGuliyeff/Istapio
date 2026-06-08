using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Category;

public record UpdateCategoryDto(
    Guid Id,
    string Name,
    Guid? ParentId = null
) : BaseDto(Id);