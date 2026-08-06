using BuildingBlocks.Domain.Events;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Domain.Contracts.DomainEvents;

public sealed record MenuItemCreatedDomainEvent : DomainEvent
{
    public required string Id { get; init; }
    public string? MenuItemName { get; init; }
    public string? MenuItemDescription { get; init; }
    public string? ImageUrl { get; init; }
    public decimal MenuItemPrice { get; init; }
    public MenuCategory Category { get; init; }
    public MenuSubCategory SubCategory { get; init; }
}