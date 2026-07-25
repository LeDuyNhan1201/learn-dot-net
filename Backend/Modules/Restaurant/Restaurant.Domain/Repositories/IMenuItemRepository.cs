using BuildingBlocks.Domain.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Repositories;

public interface IMenuItemRepository : ISqlRepository<MenuItem>
{
}