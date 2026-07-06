using BuildingBlocks.Domain.ExecutionContext;
using BuildingBlocks.Domain.ExecutionContext.Interfaces;

namespace BuildingBlocks.Infrastructure.Persistence.ExecutionContext;

public sealed class ExecutionContextAccessor : IExecutionContextAccessor
{
    private static readonly AsyncLocal<AppExecutionContext?> Context = new();

    public AppExecutionContext Current => Context.Value ?? AppExecutionContext.System;

    public void Set(AppExecutionContext context)
    {
        Context.Value = context;
    }

    public void Clear()
    {
        Context.Value = null;
    }
}