using BuildingBlocks.Domain.Models;

namespace BuildingBlocks.Domain.Abstractions.Data;

public interface IExecutionContextAccessor
{
    AppExecutionContext Current { get; }

    void Set(AppExecutionContext context);

    void Clear();
}