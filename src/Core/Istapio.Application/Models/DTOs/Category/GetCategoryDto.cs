namespace Istapio.Application.Models.DTOs.Category;

public record GetCategoryDto(
    Guid Id,
    string Name,
    Guid? ParentId
);