using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Category;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            throw new NotFoundException(nameof(Category), id);

        return Map(category);
    }

    public async Task<List<GetCategoryDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(List<GetCategoryDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<GetCategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        Category entity = new Category
        {
            Name = dto.Name,
            ParentId = dto.ParentId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<GetCategoryDto> UpdateAsync(UpdateCategoryDto dto)
    {
        Category? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Category), dto.Id);

        entity.Name = dto.Name;
        entity.ParentId = dto.ParentId;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Category? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Category), id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    private static GetCategoryDto Map(Category c)
        => new GetCategoryDto(c.Id, c.Name, c.ParentId);
}