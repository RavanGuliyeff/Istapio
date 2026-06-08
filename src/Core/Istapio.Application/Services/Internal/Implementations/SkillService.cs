using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Skill;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _repository;

    public SkillService(ISkillRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetSkillDto?> GetByIdAsync(Guid id)
    {
        var skill = await _repository.GetByIdAsync(id);
        if (skill == null)
            throw new NotFoundException(nameof(Skill), id);

        return Map(skill);
    }

    public async Task<List<GetSkillDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(List<GetSkillDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<GetSkillDto> CreateAsync(CreateSkillDto dto)
    {
        Skill entity = new Skill
        {
            Name = dto.Name
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<GetSkillDto> UpdateAsync(UpdateSkillDto dto)
    {
        Skill? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Skill), dto.Id);

        entity.Name = dto.Name;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Skill? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Skill), id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    private static GetSkillDto Map(Skill s)
        => new GetSkillDto(s.Id, s.Name);
}