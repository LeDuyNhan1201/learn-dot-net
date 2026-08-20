using BuildingBlocks.Domain.Models;

namespace BuildingBlocks.Domain.Abstractions.Data;

public interface IExecutionContextInitializer
{
    void Initialize(AppExecutionContext context);
}