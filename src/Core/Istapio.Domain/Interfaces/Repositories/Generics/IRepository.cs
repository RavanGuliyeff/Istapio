using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Istapio.Domain.Entities.Common;
using System;
using System.Linq.Expressions;

namespace Istapio.Domain.Interfaces.Repositories.Generics;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    // Query Methods
    Task<TEntity?> GetByIdAsync(
        Guid id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = false,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = false,
        CancellationToken cancellationToken = default);

    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool enableTracking = false,
        bool ignoreQueryFilters = false);

    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool enableTracking = false,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);

    Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool enableTracking = false,
        bool ignoreQueryFilters = false,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Command Methods
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
