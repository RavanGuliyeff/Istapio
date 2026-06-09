using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.JobPost;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Application.Services.Internal.Implementations;

public class JobPostService : IJobPostService
{
    private readonly IJobPostRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cache;

    private static string CacheKey(Guid id) => $"jobpost:id:{id}";
    private const string AllCacheKey = "jobposts:all";

    public JobPostService(
        IJobPostRepository repository,
        ICompanyRepository companyRepository,
        ICategoryRepository categoryRepository,
        ICacheService cache)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _cache = cache;
    }

    public async Task<GetJobPostDetailsDto> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetJobPostDetailsDto>(CacheKey(id));
        if (cached is not null) return cached;

        var jobPost = await _repository.GetByIdAsync(id
            , include: jp => jp
            .Include(jp => jp.Company)
            .Include(jp => jp.Category));

        if (jobPost == null)
            throw new NotFoundException(nameof(JobPost), id);

        var dto = MapToDetailsDto(jobPost);
        await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(15));
        return dto;
    }

    public async Task<List<GetJobPostDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetJobPostDto>>(AllCacheKey);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync(include: jp => jp
            .Include(jp => jp.Company)
            .Include(jp => jp.Category));

        var dtos = list.Select(MapToDto).ToList();
        await _cache.SetAsync(AllCacheKey, dtos, TimeSpan.FromMinutes(15));
        return dtos;
    }

    public async Task<(List<GetJobPostDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize,
            include: jp => jp
            .Include(jp => jp.Company)
            .Include(jp => jp.Category)
        );
        return (items.Select(MapToDto).ToList(), total);
    }

    public async Task<GetJobPostDto> CreateAsync(CreateJobPostDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null)
            throw new NotFoundException(nameof(Company), dto.CompanyId);

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), dto.CategoryId);

        if (await _repository.AnyAsync(j => j.Title == dto.Title && j.CompanyId == dto.CompanyId))
            throw new ConflictException($"Job post with title '{dto.Title}' already exists for this company");

        if (dto.LastDate.HasValue && dto.LastDate <= DateTime.UtcNow)
            throw new ValidationException("LastDate must be in the future");

        JobPost entity = new JobPost
        {
            Title = dto.Title,
            Description = dto.Description,
            Requirements = dto.Requirements,
            IsActive = dto.IsActive,
            ViewCount = 0,
            LastDate = dto.LastDate,
            CompanyId = dto.CompanyId,
            CategoryId = dto.CategoryId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(AllCacheKey);
        return MapToDto(entity);
    }

    public async Task<GetJobPostDto> UpdateAsync(UpdateJobPostDto dto)
    {
        JobPost? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(JobPost), dto.Id);

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), dto.CategoryId);

        if (entity.Title != dto.Title &&
            await _repository.AnyAsync(j => j.Title == dto.Title && j.CompanyId == entity.CompanyId))
            throw new ConflictException($"Job post with title '{dto.Title}' already exists for this company");

        if (dto.LastDate.HasValue && dto.LastDate <= DateTime.UtcNow)
            throw new ValidationException("LastDate must be in the future");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.Requirements = dto.Requirements;
        entity.IsActive = dto.IsActive;
        entity.LastDate = dto.LastDate;
        entity.CategoryId = dto.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(AllCacheKey);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        JobPost? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(JobPost), id);

        await _repository.SoftDeleteAsync(id);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(AllCacheKey);
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        var jobPost = await _repository.GetByIdAsync(id, enableTracking: true);

        if (jobPost is null)
            throw new NotFoundException(nameof(JobPost), id);

        jobPost.ViewCount++;

        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(AllCacheKey);
    }

    private static GetJobPostDto MapToDto(JobPost j)
        => new GetJobPostDto(
            
            Id: j.Id,
            Title: j.Title,
            IsActive: j.IsActive,
            ViewCount: j.ViewCount,
            LastDate: j.LastDate,
            CategoryId: j.CategoryId,
            CategoryName: j.Category?.Name ?? "Unknown",
            CompanyId: j.CompanyId,
            CompanyName: j.Company?.Name ?? "Unknown"
        );

    private static GetJobPostDetailsDto MapToDetailsDto(JobPost j)
        => new GetJobPostDetailsDto(
            
            Id: j.Id,
            Title: j.Title,
            Description: j.Description,
            Requirements: j.Requirements,
            IsActive: j.IsActive,
            ViewCount: j.ViewCount,
            LastDate: j.LastDate,
            CategoryId: j.CategoryId,
            CategoryName: j.Category.Name,
            CompanyId: j.CompanyId,
            CompanyName: j.Company.Name,
            CreatedAt: j.CreatedAt,
            CreatedBy: j.CreatedBy,
            UpdatedAt: j.UpdatedAt,
            UpdatedBy: j.UpdatedBy
        );
}