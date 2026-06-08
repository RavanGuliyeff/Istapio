using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Company;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;

namespace Istapio.Application.Services.Internal.Implementations;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;

    public CompanyService(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCompanyDto?> GetByIdAsync(Guid id)
    {
        var company = await _repository.GetByIdAsync(id);
        if (company == null)
            throw new NotFoundException(nameof(Company), id);

        return Map(company);
    }

    public async Task<List<GetCompanyDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(List<GetCompanyDto> Items, int TotalCount)> GetPagedAsync(int pageIndex = 1, int pageSize = 10)
    {
        var (items, total) = await _repository.GetPagedAsync(
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        var dtos = items.Select(Map).ToList();
        return (dtos, total);
    }

    public async Task<GetCompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        Company entity = new Company
        {
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            UserId = dto.UserId ?? string.Empty
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<GetCompanyDto> UpdateAsync(UpdateCompanyDto dto)
    {
        Company? entity = await _repository.GetByIdAsync(dto.Id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Company), dto.Id);

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.LogoUrl = dto.LogoUrl;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        Company? entity = await _repository.GetByIdAsync(id, enableTracking: true);
        if (entity == null)
            throw new NotFoundException(nameof(Company), id);

        await _repository.SoftDeleteAsync(id);
        await _repository.SaveChangesAsync();
    }

    private static GetCompanyDto Map(Company c)
        => new GetCompanyDto(
            c.Id,
            c.Name,
            c.Description,
            c.LogoUrl,
            c.UserId,
            c.CreatedAt,
            c.CreatedBy,
            c.UpdatedAt,
            c.UpdatedBy
        );
}