using Istapio.Application.Models.DTOs.Skill;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface ISkillService
{
    // Query
    Task<GetSkillDto?> GetByIdAsync(Guid id);
    Task<List<GetSkillDto>> GetAllAsync();
    Task<(List<GetSkillDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<GetSkillDto> CreateAsync(CreateSkillDto dto);
    Task<GetSkillDto> UpdateAsync(UpdateSkillDto dto);
    Task DeleteAsync(Guid id);
}