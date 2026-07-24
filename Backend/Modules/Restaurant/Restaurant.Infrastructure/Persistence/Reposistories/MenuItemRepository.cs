using BuildingBlocks.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Reposistories;

namespace Restaurant.Infrastructure.Persistence.Reposistories;

public class MenuItemRepository(DbContext dbContext) 
    : SqlRepository<MenuItem>(dbContext), IMenuItemRepository
{
    
}