using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Setting;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class SettingService : ISettingService
{
    private readonly ISettingRepository _repository;
    private readonly ICacheService _cache;

    private static string CacheKey(Guid id) => $"setting:id:{id}";
    private static string CacheKey(string key) => $"setting:key:{key}";
    private const string AllSettingsCacheKey = "settings:all";

    public SettingService(ISettingRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<GetSettingDto?> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetSettingDto>(CacheKey(id));
        if (cached is not null) return cached;

        var setting = await _repository.GetByIdAsync(id);
        if (setting == null)
            throw new NotFoundException(nameof(Setting), id);

        var dto = Map(setting);
        await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<GetSettingDto?> GetByKeyAsync(string key)
    {
        var cached = await _cache.GetAsync<GetSettingDto>(CacheKey(key));
        if (cached is not null) return cached;

        var setting = await _repository.GetAsync(s => s.Key == key);
        if (setting == null)
            throw new NotFoundException(nameof(Setting), key);

        var dto = Map(setting);
        await _cache.SetAsync(CacheKey(key), dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<List<GetSettingDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetSettingDto>>(AllSettingsCacheKey);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync();
        var dtos = list.Select(Map).ToList();
        await _cache.SetAsync(AllSettingsCacheKey, dtos, TimeSpan.FromMinutes(30));
        return dtos;
    }

    public async Task<(List<GetSettingDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<GetSettingDto> CreateAsync(CreateSettingDto dto)
    {
        if (await _repository.AnyAsync(s => s.Key == dto.Key))
            throw new ConflictException($"Setting with key '{dto.Key}' already exists");

        Setting entity = new Setting
        {
            Key = dto.Key,
            Value = dto.Value
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(AllSettingsCacheKey);
        return Map(entity);
    }

    public async Task<GetSettingDto> UpdateAsync(UpdateSettingDto dto)
    {
        Setting? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Setting), dto.Id);

        entity.Value = dto.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(CacheKey(entity.Key));
        await _cache.RemoveAsync(AllSettingsCacheKey);
        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Setting? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Setting), id);

        await _repository.SoftDeleteAsync(id);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(CacheKey(entity.Key));
        await _cache.RemoveAsync(AllSettingsCacheKey);
    }

    public async Task DeleteByKeyAsync(string key)
    {
        Setting? entity = await _repository.GetAsync(s => s.Key == key, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Setting), key);

        await _repository.SoftDeleteAsync(entity.Id);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(CacheKey(key));
        await _cache.RemoveAsync(AllSettingsCacheKey);
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var cached = await _cache.GetAsync<string>(CacheKey(key));
        if (cached is not null) return cached;

        Setting? entity = await _repository.GetAsync(s => s.Key == key);
        if (entity == null) return null;

        await _cache.SetAsync(CacheKey(key), entity.Value, TimeSpan.FromMinutes(30));
        return entity.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        Setting? entity = await _repository.GetAsync(s => s.Key == key, enableTracking: true);

        if (entity == null)
        {
            entity = new Setting
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = value,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(entity);
        }
        else
        {
            entity.Value = value;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
        }

        await _repository.SaveChangesAsync();
        await _cache.RemoveAsync(CacheKey(key));
        await _cache.RemoveAsync(AllSettingsCacheKey);
    }

    private static GetSettingDto Map(Setting s)
        => new GetSettingDto(
            s.Id,
            s.Key,
            s.Value,
            s.CreatedAt,
            s.CreatedBy,
            s.UpdatedAt,
            s.UpdatedBy
        );
}