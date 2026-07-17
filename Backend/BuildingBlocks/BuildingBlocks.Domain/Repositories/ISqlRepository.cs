using Ardalis.Specification;
using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.Domain.Repositories;

public interface ISqlRepository<T> where T : AuditEntity
{
    Task<T?> GetByIdAsync(params object[] keys);

    Task<List<T>> ListAsync(ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task<T?> SingleOrDefaultAsync(ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task AddAsync(T entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);
}