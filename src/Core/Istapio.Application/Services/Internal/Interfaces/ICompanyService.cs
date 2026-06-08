using Istapio.Application.Models.DTOs.Company;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface ICompanyService
{
    // Query
    Task<GetCompanyDto?> GetByIdAsync(Guid id);
    Task<List<GetCompanyDto>> GetAllAsync();
    Task<(List<GetCompanyDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<GetCompanyDto> CreateAsync(CreateCompanyDto dto);
    Task<GetCompanyDto> UpdateAsync(UpdateCompanyDto dto);
    Task DeleteAsync(Guid id);
}