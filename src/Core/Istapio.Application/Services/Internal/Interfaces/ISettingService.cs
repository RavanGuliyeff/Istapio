using Istapio.Application.Models.DTOs.Setting;
using Istapio.Application.Models.Responses;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface ISettingService
{
    // Query
    Task<GetSettingDto?> GetByIdAsync(Guid id);
    Task<GetSettingDto?> GetByKeyAsync(string key);
    Task<List<GetSettingDto>> GetAllAsync();
    Task<(List<GetSettingDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<GetSettingDto> CreateAsync(CreateSettingDto dto);
    Task<GetSettingDto> UpdateAsync(UpdateSettingDto dto);
    //Task<GetSettingDto> UpdateByKeyAsync(UpdateSettingDto dto);
    Task DeleteAsync(Guid id);
    Task DeleteByKeyAsync(string key);

    // Config helpers
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
}
