namespace Restaurant.Domain.Contracts;

public sealed record MenuItemCreatedIntegrationEvent
{
    public string? MenuItemName { get; init; }
    public string? MenuItemDescription { get; init; }
    public string? ImageUrl { get; init; }
    public decimal MenuItemPrice { get; init; }
}