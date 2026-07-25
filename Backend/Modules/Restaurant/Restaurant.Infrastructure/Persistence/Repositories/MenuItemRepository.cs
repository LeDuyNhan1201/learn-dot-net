using BuildingBlocks.Persistence.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Repositories;

namespace Restaurant.Infrastructure.Persistence.Repositories;

public class MenuItemRepository(RestaurantDbContext dbContext)
    : SqlRepository<MenuItem>(dbContext), IMenuItemRepository
{
}