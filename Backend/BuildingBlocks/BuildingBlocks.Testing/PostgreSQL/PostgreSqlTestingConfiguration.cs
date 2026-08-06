using BuildingBlocks.Domain.DbContexts;
using BuildingBlocks.Testing.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Testing.PostgreSQL;

public static class PostgreSqlTestingConfiguration
{
    public static void ConfigureTestPostgres<T>(this IServiceCollection services, PostgreSqlFixture postgres) 
        where T : DbContext, IApplicationDbContext
    {
        services.RemoveAll<DbContextOptions<T>>();
        services.AddDbContext<T>(options => { options.UseNpgsql(postgres.ConnectionString); });
    }
}