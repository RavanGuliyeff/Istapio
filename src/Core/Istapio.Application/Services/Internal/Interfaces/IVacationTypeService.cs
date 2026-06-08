using Istapio.Application.Models.DTOs.VacationType;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface IVacationTypeService
{
    // Query
    Task<GetVacationTypeDto?> GetByIdAsync(Guid id);
    Task<List<GetVacationTypeDto>> GetAllAsync();
    Task<(List<GetVacationTypeDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<GetVacationTypeDto> CreateAsync(CreateVacationTypeDto dto);
    Task<GetVacationTypeDto> UpdateAsync(UpdateVacationTypeDto dto);
    Task DeleteAsync(Guid id);
}