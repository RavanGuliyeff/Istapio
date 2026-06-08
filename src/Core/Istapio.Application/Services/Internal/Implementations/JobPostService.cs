using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.JobPost;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class JobPostService : IJobPostService
{
    private readonly IJobPostRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICategoryRepository _categoryRepository;

    public JobPostService(
        IJobPostRepository repository,
        ICompanyRepository companyRepository,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
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
        // Validate that Company exists
        var company = await _companyRepository.GetByIdAsync(dto.CompanyId);
        if (company == null)
            throw new NotFoundException(nameof(Company), dto.CompanyId);

        // Validate that Category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), dto.CategoryId);

        // Check if job post with same title for this company already exists (business rule: unique per company)
        if (await _repository.AnyAsync(j => j.Title == dto.Title && j.CompanyId == dto.CompanyId))
            throw new ConflictException($"Job post with title '{dto.Title}' already exists for this company");

        // Validate LastDate if provided
        if (dto.LastDate.HasValue && dto.LastDate <= DateTime.UtcNow)
            throw new ValidationException("LastDate must be in the future");

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

        // Validate that Category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), dto.CategoryId);

        // Check if title is being changed to a title that already exists for this company
        if (entity.Title != dto.Title && 
            await _repository.AnyAsync(j => j.Title == dto.Title && j.CompanyId == entity.CompanyId))
            throw new ConflictException($"Job post with title '{dto.Title}' already exists for this company");

        // Validate LastDate if provided
        if (dto.LastDate.HasValue && dto.LastDate <= DateTime.UtcNow)
            throw new ValidationException("LastDate must be in the future");

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