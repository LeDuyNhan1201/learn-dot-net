using BuildingBlocks.Domain.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.DbContexts;

public abstract class ApplicationDbContext<T>(DbContextOptions options) 
    : DbContext(options), IApplicationDbContext
    where T : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(T).Assembly);
    }
}