namespace BuildingBlocks.Domain.ExecutionContext.Interfaces;

public interface IExecutionContextInitializer
{
    void Initialize(AppExecutionContext context);
}