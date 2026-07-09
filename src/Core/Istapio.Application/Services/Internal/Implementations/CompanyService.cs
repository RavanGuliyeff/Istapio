using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Company;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Application.Utilities.Constants;
using Istapio.Domain.Constants;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Application.Services.Internal.Implementations;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly ICacheService _cache;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;

    public CompanyService(ICompanyRepository repository, ICacheService cache, ICurrentUserService currentUserService, IFileService fileService)
    {
        _repository = repository;
        _cache = cache;
        _currentUserService = currentUserService;
        _fileService = fileService;
    }

    public async Task<GetCompanyDetailsDto> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<GetCompanyDetailsDto>(CacheKeys.Companies.ById(id));
        if (cached is not null) return cached;

        var company = await _repository.GetByIdAsync(id,
            include: c => c
            .Include(c => c.User)
            .Include(c => c.JobPosts)
                .ThenInclude(jp => jp.Category)
            .Include(c => c.JobPosts)
                .ThenInclude(jp => jp.VacationType)
            );

        if (company == null)
            throw new NotFoundException(nameof(Company), id);

        var dto = MapToDetailsDto(company);
        await _cache.SetAsync(CacheKeys.Companies.ById(id), dto, TimeSpan.FromMinutes(30));
        return dto;
    }

    public async Task<List<GetCompanyDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<GetCompanyDto>>(CacheKeys.Companies.All);
        if (cached is not null) return cached;

        var list = await _repository.GetAllAsync(include: c => c
            .Include(c => c.User));
        var dtos = list.Select(MapToDto).ToList();
        await _cache.SetAsync(CacheKeys.Companies.All, dtos, TimeSpan.FromMinutes(30));
        return dtos;
    }

    public async Task<(List<GetCompanyDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize,
            include: c => c
            .Include(c => c.User));
        return (items.Select(MapToDto).ToList(), total);
    }

    public async Task<GetCompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new ValidationException("UserId is required");

        if (await _repository.AnyAsync(c => c.Name == dto.Name && c.UserId == userId))
            throw new ConflictException($"Company with name '{dto.Name}' already exists for this user");

        using var stream = dto.Logo.OpenReadStream();
        var uploadResult = await _fileService.UploadAsync(
            stream,
            dto.Logo.FileName,
            S3Folders.Companies,
            dto.Logo.ContentType);

        Company entity = new Company
        {
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = uploadResult.Url,
            UserId = userId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKeys.Companies.All);
        return MapToDto(entity);
    }

    public async Task<GetCompanyDto> UpdateAsync(UpdateCompanyDto dto)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new ValidationException("UserId is required");

        Company? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null || entity.UserId != userId)
            throw new NotFoundException($"{nameof(Company)} not found or you do not have permission to update it");

        if (entity.Name != dto.Name &&
            await _repository.AnyAsync(c => c.Name == dto.Name))
            throw new ConflictException($"Company with name '{dto.Name}' already exists ");

        if (dto.Logo is not null)
        {
            var oldLogoUrl = entity.LogoUrl;

            using var stream = dto.Logo.OpenReadStream();
            var uploadResult = await _fileService.UploadAsync(
                stream,
                dto.Logo.FileName,
                S3Folders.Companies,
                dto.Logo.ContentType);

            entity.LogoUrl = uploadResult.Url;

            if (!string.IsNullOrWhiteSpace(oldLogoUrl))
            {
                var oldKey = ExtractKeyFromUrl(oldLogoUrl);
                if (oldKey is not null)
                    await _fileService.DeleteAsync(oldKey);
            }
        }

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKeys.Companies.ById(entity.Id));
        await _cache.RemoveAsync(CacheKeys.Companies.All);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
            throw new ValidationException("UserId is required");

        Company? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null || (entity.UserId != userId && !_currentUserService.IsInAnyRole(Roles.SuperAdmin, Roles.Admin, Roles.Moderator)))
            throw new NotFoundException($"{nameof(Company)} not found or you do not have permission to delete it");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(entity.LogoUrl))
        {
            var key = ExtractKeyFromUrl(entity.LogoUrl);
            if (key is not null)
            {
                try
                {
                    await _fileService.DeleteAsync(key);
                }
                catch (InvalidOperationException)
                {
                    // logging here
                }
            }
        }

        await _cache.RemoveAsync(CacheKeys.Companies.ById(id));
        await _cache.RemoveAsync(CacheKeys.Companies.All);
    }

    private static string? ExtractKeyFromUrl(string url)
    {
        // https://{bucket}.s3.{region}.amazonaws.com/{key}
        var uri = new Uri(url);
        return uri.AbsolutePath.TrimStart('/');
    }

    private static GetCompanyDto MapToDto(Company c)
    => new(
        Id: c.Id,
        Name: c.Name,
        LogoUrl: c.LogoUrl,
        UserId: c.UserId,
        UserName: c.User?.UserName ?? "Unknown"
    );
    private static GetCompanyDetailsDto MapToDetailsDto(Company c)
    => new(
        Id: c.Id,
        Name: c.Name,
        Description: c.Description,
        LogoUrl: c.LogoUrl,
        UserId: c.UserId,
        UserName: c.User.UserName!,
        JobPosts: c.JobPosts.Select(j => new GetCompanyJobPostDto(
            Id: j.Id,
            Title: j.Title,
            CategoryId: j.CategoryId,
            CategoryName: j.Category.Name,
            VacationTypeId: j.VacationTypeId,
            VacationTypeName: j.VacationType.Name,
            IsActive: j.IsActive,
            ViewCount: j.ViewCount
        )).ToList(),
        CreatedAt: c.CreatedAt,
        CreatedBy: c.CreatedBy,
        UpdatedAt: c.UpdatedAt,
        UpdatedBy: c.UpdatedBy
    );
}