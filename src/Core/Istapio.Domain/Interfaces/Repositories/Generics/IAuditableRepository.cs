using Microsoft.EntityFrameworkCore.Query;
using Istapio.Domain.Entities.Common;
using System.Linq.Expressions;

namespace Istapio.Domain.Interfaces.Repositories.Generics;

public interface IAuditableRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseAuditableEntity
{
    Task<TEntity?> GetByIdIncludingDeletedAsync(
        Guid id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllIncludingDeletedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetDeletedOnlyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SoftDeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task RestoreAsync(Guid id, CancellationToken cancellationToken = default);
    Task RestoreRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task HardDeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetByCreatorAsync(string createdBy, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetByModifierAsync(string updatedBy, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetModifiedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
