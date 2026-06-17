using MediatR;

namespace Restaurant.Application.Contracts;

public interface IMenuItemCommand
{
    public record Create : IRequest<string>
    {
        public string? MenuItemName  { get; init; }
        public string? MenuItemDescription  { get; init; }
        public string? ImageUrl  { get; init; }
        public decimal MenuItemPrice  { get; init; }
    }
}