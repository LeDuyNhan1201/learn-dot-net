using BuildingBlocks.Domain.ExecutionContext;
using BuildingBlocks.Domain.ExecutionContext.Interfaces;

namespace BuildingBlocks.Infrastructure.Persistence.ExecutionContext;

public sealed class ExecutionContextInitializer(IExecutionContextAccessor accessor) : IExecutionContextInitializer
{
    public void Initialize(AppExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        accessor.Set(context);
    }
}