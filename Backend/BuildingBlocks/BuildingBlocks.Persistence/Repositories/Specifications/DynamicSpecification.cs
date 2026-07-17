using Ardalis.Specification;
using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.Persistence.Repositories.Specifications;

internal sealed class DynamicSpecification<T> : Specification<T>
    where T : AuditEntity
{
    public DynamicSpecification(Action<ISpecificationBuilder<T>> configure)
    {
        configure(Query);
    }
}