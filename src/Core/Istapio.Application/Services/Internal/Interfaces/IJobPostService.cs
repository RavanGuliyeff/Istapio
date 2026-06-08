using Istapio.Application.Models.DTOs.JobPost;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface IJobPostService
{
    // Query
    Task<GetJobPostDto?> GetByIdAsync(Guid id);
    Task<List<GetJobPostDto>> GetAllAsync();
    Task<(List<GetJobPostDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10);

    // Command
    Task<GetJobPostDto> CreateAsync(CreateJobPostDto dto);
    Task<GetJobPostDto> UpdateAsync(UpdateJobPostDto dto);
    Task DeleteAsync(Guid id);
}