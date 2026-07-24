using BuildingBlocks.Domain.Events;

namespace Restaurant.Domain.Contracts;

public interface IMenuItemDomainEvent
{
    public record Created : DomainEvent
    {
        public required string Id { get; init; }
        public string? MenuItemName { get; init; }
        public string? MenuItemDescription { get; init; }
        public string? ImageUrl { get; init; }
        public decimal MenuItemPrice { get; init; }
    }
}