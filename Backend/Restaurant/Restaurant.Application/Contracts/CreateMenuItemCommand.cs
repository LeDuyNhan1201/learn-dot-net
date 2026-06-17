using MediatR;

namespace Restaurant.Application.Contracts;

public sealed record CreateMenuItemCommand(
    string Name,
    string Description,
    string ImageUrl,
    decimal Price) : IRequest<Guid>;