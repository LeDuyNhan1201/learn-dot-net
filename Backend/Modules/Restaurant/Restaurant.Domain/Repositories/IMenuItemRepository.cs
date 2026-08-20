using BuildingBlocks.Domain.Abstractions.Data;
using Restaurant.Domain.Entities;

namespace Restaurant.Domain.Repositories;

public interface IMenuItemRepository : ISqlRepository<MenuItem>
{
}