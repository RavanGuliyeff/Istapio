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

    public async Task<GetCategoryDetailsDto> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetCategoryDetailsDto>(CacheKey(id));
        if (cached is not null)
            return cached;

        var category = await _repository.GetByIdAsync(id,
            include: c => c
            .Include(x => x.SubCategories)
            .Include(x => x.JobPosts)
                .ThenInclude(jp => jp.Company)
            .Include(x => x.JobPosts)
                .ThenInclude(jp => jp.VacationType)
            );
        if (category == null)
            throw new NotFoundException(nameof(Category), id);

        var dto = MapToDetailsDto(category);

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
        var dtos = list.Select(MapToDto).ToList();

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

        var dtos = items.Select(MapToDto).ToList();

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

        return MapToDto(entity);
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

        return MapToDto(entity);
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

    private static GetCategoryDto MapToDto(Category c)
        => new(
            Id: c.Id,
            Name: c.Name,
            ParentId: c.ParentId,
            SubCategories: c.SubCategories.Select(MapToDto).ToList() ?? new List<GetCategoryDto>()
        );
    private static GetCategoryDetailsDto MapToDetailsDto(Category c)
        => new(
            Id: c.Id,
            Name: c.Name,
            JobPostsCount: c.JobPosts?.Count ?? 0,
            ParentId: c.ParentId,
            SubCategories: c.SubCategories.Select(MapToDto).ToList() ?? new List<GetCategoryDto>(),
            JobPosts: c.JobPosts!.Select(jp => new GetCategoryJobPostDto(
                Id: jp.Id,
                Title: jp.Title,
                CompanyId: jp.CompanyId,
                CompanyName: jp.Company.Name,
                VacationTypeId: jp.VacationTypeId,
                VacationTypeName: jp.VacationType.Name,
                IsActive: jp.IsActive,
                ViewCount: jp.ViewCount
                )).ToList() ?? new List<GetCategoryJobPostDto>()
           );

}
