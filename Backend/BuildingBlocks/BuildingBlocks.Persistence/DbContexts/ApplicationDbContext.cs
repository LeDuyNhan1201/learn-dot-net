using BuildingBlocks.Domain.Abstractions.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.DbContexts;

public abstract class ApplicationDbContext<T>(DbContextOptions<T> options)
    : DbContext(options), IApplicationDbContext
    where T : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(T).Assembly);
    }
}