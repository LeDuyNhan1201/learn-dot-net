using Ardalis.Specification;
using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.Persistence.Repositories.Specifications;

public static class SpecificationFactory
{
    public static ISpecification<T> Create<T>(Action<ISpecificationBuilder<T>> configure)
        where T : AuditEntity
    {
        return new DynamicSpecification<T>(configure);
    }
}