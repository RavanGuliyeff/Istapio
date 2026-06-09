using Istapio.Application.Models.DTOs.JobPost;

namespace Istapio.Application.Models.DTOs.Category;

public record GetCategoryDetailsDto(
    Guid Id,
    string Name,
    long JobPostsCount,
    Guid? ParentId,
    ICollection<GetCategoryDto> SubCategories,
    ICollection<GetCategoryJobPostDto> JobPosts
);