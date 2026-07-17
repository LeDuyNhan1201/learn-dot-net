using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Repositories;
using BuildingBlocks.SharedKernel.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.Repositories;

public class SqlRepository<T>(DbContext context) : IPaginationRepository<T>
    where T : AuditEntity
{
    protected DbSet<T> DbSet => context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(params object[] keys)
    {
        return await DbSet.FindAsync(keys);
    }

    public virtual async Task<List<T>> ListAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<T?> SingleOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .AnyAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification)
            .CountAsync(cancellationToken);
    }

    public virtual async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public virtual async Task<PagedResults.Offset<T>> OffsetAsync(
        OffsetQuery<T> query,
        CancellationToken cancellationToken = default)
    {
        var countQuery =
            SpecificationEvaluator.Default.GetQuery(
                DbSet.AsQueryable(),
                query.Specification,
                true);

        var total = await countQuery.CountAsync(cancellationToken);

        var items = await ApplySpecification(query.Specification)
            .ToListAsync(cancellationToken);

        return new PagedResults.Offset<T>
        {
            Items = items,
            Total = total,
            Page = query.Page,
            Size = query.Size
        };
    }

    protected virtual IQueryable<T> ApplySpecification(
        ISpecification<T> specification)
    {
        return SpecificationEvaluator.Default
            .GetQuery(DbSet.AsQueryable(), specification);
    }
}