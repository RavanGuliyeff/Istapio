using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Company;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly ICacheService _cache;

    private static string CacheKey(Guid id) => $"company:id:{id}";
    private const string AllCacheKey = "companies:all";

    public CompanyService(ICompanyRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<GetCompanyDto?> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetCompanyDto>(CacheKey(id));
        if (cached is not null) return cached;

        var company = await _repository.GetByIdAsync(id);
        if (company == null)
            throw new NotFoundException(nameof(Company), id);

        var dto = Map(company);
        await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<List<GetCompanyDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetCompanyDto>>(AllCacheKey);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync();
        var dtos = list.Select(Map).ToList();
        await _cache.SetAsync(AllCacheKey, dtos, TimeSpan.FromMinutes(30));
        return dtos;
    }

    public async Task<(List<GetCompanyDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(pageIndex: pageIndex, pageSize: pageSize);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<GetCompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
            throw new ValidationException("UserId is required");

        if (await _repository.AnyAsync(c => c.Name == dto.Name && c.UserId == dto.UserId))
            throw new ConflictException($"Company with name '{dto.Name}' already exists for this user");

        Company entity = new Company
        {
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            UserId = dto.UserId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(AllCacheKey);
        return Map(entity);
    }

    public async Task<GetCompanyDto> UpdateAsync(UpdateCompanyDto dto)
    {
        Company? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Company), dto.Id);

        if (entity.Name != dto.Name &&
            await _repository.AnyAsync(c => c.Name == dto.Name && c.UserId == entity.UserId))
            throw new ConflictException($"Company with name '{dto.Name}' already exists for this user");

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.LogoUrl = dto.LogoUrl;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(AllCacheKey);
        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Company? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Company), id);

        await _repository.SoftDeleteAsync(id);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(AllCacheKey);
    }

    private static GetCompanyDto Map(Company c)
        => new(c.Id, c.Name, c.Description, c.LogoUrl, c.UserId,
               c.CreatedAt, c.CreatedBy, c.UpdatedAt, c.UpdatedBy);
}