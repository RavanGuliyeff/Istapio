using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Category;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Application.Services.Internal.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ICacheService _cache;

    private static string CacheKey(Guid id) => $"category:id:{id}";
    private const string AllCategoriesCacheKey = "categories:all";

    public CategoryService(
        ICategoryRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<GetCategoryDto?> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetCategoryDto>(CacheKey(id));
        if (cached is not null)
            return cached;

        var category = await _repository.GetByIdAsync(id, include: c => c.Include(x => x.SubCategories));
        if (category == null)
            throw new NotFoundException(nameof(Category), id);

        var dto = Map(category);

        await _cache.SetAsync(
            CacheKey(id),
            dto,
            TimeSpan.FromMinutes(30));

        return dto;
    }

    public async Task<List<GetCategoryDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetCategoryDto>>(AllCategoriesCacheKey);
        if (cached is not null)
            return cached;

        var list = await _repository.GetAllAsync(include: c => c.Include(x => x.SubCategories));
        var dtos = list.Select(Map).ToList();

        await _cache.SetAsync(
            AllCategoriesCacheKey,
            dtos,
            TimeSpan.FromMinutes(30));

        return dtos;
    }

    public async Task<(List<GetCategoryDto> Items, int TotalCount)> GetPagedAsync(
        int pageIndex = 1,
        int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize, 
            include: c => c.Include(x => x.SubCategories)
        );

        var dtos = items.Select(Map).ToList();

        return (dtos, total);
    }

    public async Task<GetCategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        if (await _repository.AnyAsync(
        c => c.Name.ToLower() == dto.Name.ToLower()))
        {
            throw new ConflictException(
                $"Category with name '{dto.Name}' already exists");
        }

        if (dto.ParentId.HasValue)
        {
            var parent = await _repository.GetByIdAsync(dto.ParentId.Value);

            if (parent == null)
                throw new NotFoundException(nameof(Category), dto.ParentId.Value);
        }

        Category entity = new()
        {
            Name = dto.Name,
            ParentId = dto.ParentId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(AllCategoriesCacheKey);

        return Map(entity);
    }

    public async Task<GetCategoryDto> UpdateAsync(UpdateCategoryDto dto)
    {
        Category? entity = await _repository.GetByIdAsync(
            dto.Id,
            enableTracking: true);

        if (entity == null)
            throw new NotFoundException(nameof(Category), dto.Id);

        if (!string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase) &&
            await _repository.AnyAsync(
                c => c.Id != dto.Id &&
                c.Name.ToLower() == dto.Name.ToLower()))
        {
            throw new ConflictException(
                $"Category with name '{dto.Name}' already exists");
        }

        if (dto.ParentId.HasValue)
        {
            if (dto.ParentId == entity.Id)
                throw new ConflictException("A category cannot be its own parent");

            var parent = await _repository.GetByIdAsync(dto.ParentId.Value);

            if (parent == null)
                throw new NotFoundException(nameof(Category), dto.ParentId.Value);
        }

        entity.Name = dto.Name;
        entity.ParentId = dto.ParentId;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(AllCategoriesCacheKey);

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Category? entity = await _repository.GetByIdAsync(
            id,
            enableTracking: true);

        if (entity == null)
            throw new NotFoundException(nameof(Category), id);

        if (await _repository.AnyAsync(x => x.ParentId == id))
        {
            throw new ConflictException(
                "Cannot delete category because it has child categories");
        }

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(AllCategoriesCacheKey);
    }

    private static GetCategoryDto Map(Category c)
        => new(
            c.Id,
            c.Name,
            c.ParentId,
            c.SubCategories.Select(Map).ToList() ?? new List<GetCategoryDto>()
        );
}