using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Istapio.Domain.Entities.Common;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories.Generics;
using Istapio.Persistence.Contexts;
using System.Linq.Expressions;

namespace Istapio.Persistence.Implementations.Generics;

public class AuditableRepository<TEntity> : Repository<TEntity>, IAuditableRepository<TEntity>
    where TEntity : BaseAuditableEntity
{

    private readonly ICurrentUserService _currentUserService;
    public AuditableRepository(AppDbContext context, ICurrentUserService currentUserService) : base(context)
    {
        _currentUserService = currentUserService;
    }

    // Silinmiş entity-ləri də gətir
    public async Task<TEntity?> GetByIdIncludingDeletedAsync(
        Guid id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.IgnoreQueryFilters();

        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<TEntity>> GetAllIncludingDeletedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.IgnoreQueryFilters().AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        if (include != null)
            query = include(query);

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetDeletedOnlyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.IgnoreQueryFilters().Where(e => e.IsDeleted);

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }

    // Soft Delete
    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, enableTracking: true, cancellationToken: cancellationToken);
        if (entity != null && !entity.IsDeleted)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _currentUserService.UserId;
            // DeletedBy DbContext-in SaveChangesAsync-də set olunacaq (CurrentUserService vasitəsilə)
        }
    }

    public async Task SoftDeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet
            .Where(e => ids.Contains(e.Id) && !e.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }

    // Restore (Soft delete-i geri al)
    public async Task RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdIncludingDeletedAsync(id, cancellationToken: cancellationToken);
        if (entity != null && entity.IsDeleted)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedBy = null;
        }
    }

    public async Task RestoreRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet
            .IgnoreQueryFilters()
            .Where(e => ids.Contains(e.Id) && e.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedBy = null;
        }
    }

    // Hard Delete (database-dən tamamilə sil)
    public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdIncludingDeletedAsync(id, cancellationToken: cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task HardDeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet
            .IgnoreQueryFilters()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(entities);
    }

    // Audit Queries
    public async Task<List<TEntity>> GetByCreatorAsync(string createdBy, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.CreatedBy == createdBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetByModifierAsync(string updatedBy, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UpdatedBy == updatedBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetCreatedBetweenAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetModifiedBetweenAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UpdatedAt != null && e.UpdatedAt >= startDate && e.UpdatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }
}