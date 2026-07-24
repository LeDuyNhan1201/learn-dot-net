namespace Restaurant.Domain.Contracts;

public interface IMenuItemIntegrationEvent
{
    public record Created
    {
        public string? MenuItemName { get; init; }
        public string? MenuItemDescription { get; init; }
        public string? ImageUrl { get; init; }
        public decimal MenuItemPrice { get; init; }
    }
}