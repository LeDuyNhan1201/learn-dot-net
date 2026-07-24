using BuildingBlocks.Domain.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Reposistories;

public interface IMenuItemRepository : ISqlRepository<MenuItem>
{
    
}