using Istapio.Application.Models.DTOs.Category;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface ICategoryService
{
    // Query
    Task<GetCategoryDetailsDto> GetByIdAsync(Guid id);
    Task<List<GetCategoryDto>> GetAllAsync();
    Task<(List<GetCategoryDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<GetCategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<GetCategoryDto> UpdateAsync(UpdateCategoryDto dto);
    Task DeleteAsync(Guid id);
}
