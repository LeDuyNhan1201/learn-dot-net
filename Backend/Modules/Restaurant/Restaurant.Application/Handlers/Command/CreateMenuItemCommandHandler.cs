using BuildingBlocks.Domain.Repositories.UnitOfWork;
using MediatR;
using Restaurant.Domain.Contracts.Commands;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Repositories;

namespace Restaurant.Application.Handlers.Command;

public sealed class CreateMenuItemCommandHandler(
    IMenuItemRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateMenuItemCommand, string>
{
    public async Task<string> Handle(
        CreateMenuItemCommand command,
        CancellationToken cancellationToken)
    {
        var entity = MenuItem.Create(command);

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id.ToString();
    }
}