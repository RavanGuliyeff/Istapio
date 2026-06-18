namespace Istapio.Application.Models.DTOs.Category;

public record CreateCategoryDto(
    string Name,
    Guid? ParentId = null
);