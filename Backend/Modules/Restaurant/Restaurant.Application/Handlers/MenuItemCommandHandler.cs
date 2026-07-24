using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Repositories.UnitOfWork;
using MediatR;
using Restaurant.Domain.Contracts;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Reposistories;

namespace Restaurant.Application.Handlers;

public static class MenuItemCommandHandler
{
    public sealed class Create(
        IMenuItemRepository repository,
        IUnitOfWork unitOfWork) 
        : IRequestHandler<IMenuItemCommand.Create, string>
    {
        public async Task<string> Handle(
            IMenuItemCommand.Create command,
            CancellationToken cancellationToken)
        {
            var entity = MenuItem.Create(command);
            
            await repository.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return entity.Id.ToString();
        }
    }
}