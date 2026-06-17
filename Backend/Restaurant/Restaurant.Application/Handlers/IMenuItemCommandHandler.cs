using MediatR;
using Restaurant.Application.Contracts;

namespace Restaurant.Application.Handlers;

public interface IMenuItemCommandHandler
{
    public sealed class Create : IRequestHandler<IMenuItemCommand.Create, string>
    {
        public Task<string> Handle(
            IMenuItemCommand.Create command,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}