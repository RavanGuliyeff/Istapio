using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.JobPost;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class JobPostService : IJobPostService
{
    private readonly IJobPostRepository _repository;

    public JobPostService(IJobPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetJobPostDto?> GetByIdAsync(Guid id)
    {
        var jobPost = await _repository.GetByIdAsync(id);
        if (jobPost == null)
            throw new NotFoundException(nameof(JobPost), id);

        return Map(jobPost);
    }

    public async Task<List<GetJobPostDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(List<GetJobPostDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<GetJobPostDto> CreateAsync(CreateJobPostDto dto)
    {
        JobPost entity = new JobPost
        {
            Title = dto.Title,
            Description = dto.Description,
            Requirements = dto.Requirements,
            IsActive = dto.IsActive,
            ViewCount = 0,
            LastDate = dto.LastDate,
            CompanyId = dto.CompanyId,
            CategoryId = dto.CategoryId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<GetJobPostDto> UpdateAsync(UpdateJobPostDto dto)
    {
        JobPost? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(JobPost), dto.Id);

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.Requirements = dto.Requirements;
        entity.IsActive = dto.IsActive;
        entity.LastDate = dto.LastDate;
        entity.CategoryId = dto.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        JobPost? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(JobPost), id);

        await _repository.SoftDeleteAsync(id);
        await _repository.SaveChangesAsync();
    }

    private static GetJobPostDto Map(JobPost j)
        => new GetJobPostDto(
            j.Id,
            j.Title,
            j.Description,
            j.Requirements,
            j.IsActive,
            j.ViewCount,
            j.LastDate,
            j.CompanyId,
            j.CategoryId,
            j.CreatedAt,
            j.CreatedBy,
            j.UpdatedAt,
            j.UpdatedBy
        );
}