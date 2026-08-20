using BuildingBlocks.Domain.Abstractions.Data;
using BuildingBlocks.Domain.Models;

namespace BuildingBlocks.Persistence.ExecutionContext;

public sealed class ExecutionContextInitializer(IExecutionContextAccessor accessor) : IExecutionContextInitializer
{
    public void Initialize(AppExecutionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        accessor.Set(context);
    }
}