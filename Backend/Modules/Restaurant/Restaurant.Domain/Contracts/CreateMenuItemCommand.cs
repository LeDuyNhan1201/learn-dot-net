using MediatR;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Domain.Contracts;

public record CreateMenuItemCommand : IRequest<string>
{
    public string? MenuItemName { get; init; }
    public string? MenuItemDescription { get; init; }
    public string? ImageUrl { get; init; }
    public decimal MenuItemPrice { get; init; }
    public MenuCategory Category { get; init; }
    public MenuSubCategory SubCategory { get; init; }
}