using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.VacationType;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class VacationTypeService : IVacationTypeService
{
    private readonly IVacationTypeRepository _repository;

    public VacationTypeService(IVacationTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetVacationTypeDto?> GetByIdAsync(Guid id)
    {
        var vacationType = await _repository.GetByIdAsync(id);
        if (vacationType == null)
            throw new NotFoundException(nameof(VacationType), id);

        return Map(vacationType);
    }

    public async Task<List<GetVacationTypeDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(List<GetVacationTypeDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<GetVacationTypeDto> CreateAsync(CreateVacationTypeDto dto)
    {
        VacationType entity = new VacationType
        {
            Name = dto.Name
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<GetVacationTypeDto> UpdateAsync(UpdateVacationTypeDto dto)
    {
        VacationType? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(VacationType), dto.Id);

        entity.Name = dto.Name;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        VacationType? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(VacationType), id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    private static GetVacationTypeDto Map(VacationType v)
        => new GetVacationTypeDto(v.Id, v.Name);
}