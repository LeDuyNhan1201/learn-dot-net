using BuildingBlocks.Domain.Contracts;
using BuildingBlocks.Domain.DbContexts;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Persistence.Repositories;

public sealed class UnitOfWork(
    IApplicationDbContext context, 
    IDomainEventExecutor executor) 
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(x => x.DequeueEvents())
            .ToList();

        await executor.ExecuteAsync(domainEvents, cancellationToken);

        return await context.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return context.Database.BeginTransactionAsync(cancellationToken);
    }
}