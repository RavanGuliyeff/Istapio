using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Company;
using Istapio.Application.Models.DTOs.Skill;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Application.Services.Internal.Implementations;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _repository;
    private readonly ICacheService _cache;

    private static string CacheKey(Guid id) => $"skill:id:{id}";
    private const string AllCacheKey = "skills:all";

    public SkillService(ISkillRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<GetSkillDetailsDto> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetSkillDetailsDto>(CacheKey(id));
        if (cached is not null) return cached;

        var skill = await _repository.GetByIdAsync(id,
            include: s => s
                .Include(s => s.JobPostSkills)
                    .ThenInclude(sj => sj.JobPost)
                .Include(s => s.UserSkills)
                    .ThenInclude(su => su.User));
        if (skill == null)
            throw new NotFoundException(nameof(Skill), id);

        var dto = MapToDetailsDto(skill);
        await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<List<GetSkillDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetSkillDto>>(AllCacheKey);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync();
        var dtos = list.Select(MapToDto).ToList();
        await _cache.SetAsync(AllCacheKey, dtos, TimeSpan.FromMinutes(30));
        return dtos;
    }

    public async Task<(List<GetSkillDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(pageIndex: pageIndex, pageSize: pageSize);
        return (items.Select(MapToDto).ToList(), total);
    }

    public async Task<GetSkillDto> CreateAsync(CreateSkillDto dto)
    {
        if (await _repository.AnyAsync(s => s.Name == dto.Name))
            throw new ConflictException($"Skill with name '{dto.Name}' already exists");

        Skill entity = new Skill { Name = dto.Name };
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(AllCacheKey);
        return MapToDto(entity);
    }

    public async Task<GetSkillDto> UpdateAsync(UpdateSkillDto dto)
    {
        Skill? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Skill), dto.Id);

        if (entity.Name != dto.Name && await _repository.AnyAsync(s => s.Name == dto.Name))
            throw new ConflictException($"Skill with name '{dto.Name}' already exists");

        entity.Name = dto.Name;
        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(entity.Id));
        await _cache.RemoveAsync(AllCacheKey);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Skill? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Skill), id);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKey(id));
        await _cache.RemoveAsync(AllCacheKey);
    }

    private static GetSkillDto MapToDto(Skill s) => new(s.Id, s.Name);
    private static GetSkillDetailsDto MapToDetailsDto(Skill s)
        => new GetSkillDetailsDto(
            Id: s.Id,
            Name: s.Name,
            JobPosts: s.JobPostSkills.Select(sj => new GetSkillJobPostDto(
            Id: sj.JobPostId,
            JobPostTitle: sj.JobPost.Title)).ToList(),
            Users: s.UserSkills.Select(su => new GetSkillUserDto(
            Id: su.UserId,
            UserName: su.User.UserName!)).ToList()

        );
}