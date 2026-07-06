using BuildingBlocks.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence;

public class RestaurantDbContext : ApplicationDbContext<RestaurantDbContext>
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<BillItem> BillItems => Set<BillItem>();
}