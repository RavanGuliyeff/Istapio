using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Setting;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
//using Istapio.Domain.Interfaces.Repositories.Generics;

namespace Istapio.Application.Services.Internal.Implementations;

public class SettingService : ISettingService
{
    private readonly ISettingRepository _repository;

    public SettingService(ISettingRepository repository)
    {
        _repository = repository;
    }
    public async Task<GetSettingDto?> GetByIdAsync(Guid id)
    {
        var setting = await _repository.GetByIdAsync(id);
        if (setting == null)
            throw new NotFoundException(nameof(Setting), id);

        return Map(setting);
    }

    public async Task<GetSettingDto?> GetByKeyAsync(string key)
    {
        var setting = await _repository.GetAsync(s => s.Key == key);
        if (setting == null)
            throw new NotFoundException(nameof(Setting), key);

        return Map(setting);
    }

    public async Task<List<GetSettingDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
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
        return Map(entity);
    }

    public async Task<GetSettingDto> UpdateByKeyAsync(UpdateSettingDto dto)
    {
        Setting? entity = await _repository.GetAsync(s => s.Key == dto.Key, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Setting), dto.Key);

        entity.Value = dto.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Setting? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Setting), id);

        await _repository.SoftDeleteAsync(id);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteByKeyAsync(string key)
    {
        Setting? entity = await _repository.GetAsync(s => s.Key == key, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Setting), key);

        await _repository.SoftDeleteAsync(entity.Id);
        await _repository.SaveChangesAsync();
    }

    public async Task<string?> GetValueAsync(string key)
    {
        Setting? entity = await _repository.GetAsync(s => s.Key == key);
        if (entity == null)
            return null;

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
