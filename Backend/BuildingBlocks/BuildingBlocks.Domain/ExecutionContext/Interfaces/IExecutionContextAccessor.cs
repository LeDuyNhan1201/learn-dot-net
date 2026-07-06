namespace BuildingBlocks.Domain.ExecutionContext.Interfaces;

public interface IExecutionContextAccessor
{
    AppExecutionContext Current { get; }

    void Set(AppExecutionContext context);

    void Clear();
}