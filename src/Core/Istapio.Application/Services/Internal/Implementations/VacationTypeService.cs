using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.VacationType;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class VacationTypeService : IVacationTypeService
{
    private readonly IVacationTypeRepository _repository;
    private readonly ICacheService _cache;

    private static string CacheKey(Guid id) => $"vacationtype:id:{id}";
    private const string AllCacheKey = "vacationtypes:all";

    public VacationTypeService(IVacationTypeRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<GetVacationTypeDto?> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetVacationTypeDto>(CacheKey(id));
        if (cached is not null) return cached;

        var vacationType = await _repository.GetByIdAsync(id);
        if (vacationType == null)
            throw new NotFoundException(nameof(VacationType), id);

        var dto = Map(vacationType);
        await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<List<GetVacationTypeDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetVacationTypeDto>>(AllCacheKey);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync();
        var dtos = list.Select(Map).ToList();
        await _cache.SetAsync(AllCacheKey, dtos, TimeSpan.FromMinutes(30));
        return dtos;
    }

    public async Task<(List<GetVacationTypeDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(pageIndex: pageIndex, pageSize: pageSize);
        return (items.Select(Map).ToList(), total);
    }

    public async Task<GetVacationTypeDto> CreateAsync(CreateVacationTypeDto dto)
    {
        if (await _repository.AnyAsync(v => v.Name == dto.Name))
            throw new ConflictException($"Vacation type with name '{dto.Name}' already exists");

        VacationType entity = new VacationType { Name = dto.Name };
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(AllCacheKey);
        return Map(entity);
    }

    public async Task<GetVacationTypeDto> UpdateAsync(UpdateVacationTypeDto dto)
    {
        VacationType? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(VacationType), dto.Id);

        if (entity.Name != dto.Name && await _repository.AnyAsync(v => v.Name == dto.Name))
            throw new ConflictException($"Vacation type with name '{dto.Name}' already exists");

        entity.Name = dto.Name;
        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(AllCacheKey);
        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        VacationType? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(VacationType), id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(AllCacheKey);
    }

    private static GetVacationTypeDto Map(VacationType v) => new(v.Id, v.Name);
}