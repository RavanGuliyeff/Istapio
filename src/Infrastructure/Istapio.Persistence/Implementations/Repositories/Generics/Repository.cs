using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Istapio.Domain.Entities.Common;
using Istapio.Domain.Interfaces.Repositories.Generics;
using Istapio.Persistence.Contexts;
using System.Linq.Expressions;

namespace Istapio.Persistence.Implementations.Generics;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (!enableTracking)
            query = query.AsNoTracking();

        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (!enableTracking)
            query = query.AsNoTracking();

        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool enableTracking = false,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;

        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();

        if (!enableTracking)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        if (include != null)
            query = include(query);

        if (orderBy != null)
            query = orderBy(query);

        return query;
    }

    public virtual async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool enableTracking = false,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(predicate, include, orderBy, enableTracking, ignoreQueryFilters);
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool enableTracking = false,
        bool ignoreQueryFilters = false,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();

        if (!enableTracking)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (include != null)
            query = include(query);

        if (orderBy != null)
            query = orderBy(query);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}