using Ardalis.Specification;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.SharedKernel.DTOs;

namespace BuildingBlocks.Domain.Repositories;

public interface IPaginationRepository<T> : ISqlRepository<T>
    where T : AuditEntity
{
    Task<PagedResults.Offset<T>> OffsetAsync(
        OffsetQuery<T> query,
        CancellationToken cancellationToken = default);
}

public sealed record OffsetQuery<T>(
    ISpecification<T> Specification,
    int Page = 1,
    int Size = 20)
    where T : class
{
    public int Page { get; init; } = Page < 1 ? 1 : Page;

    public int Size { get; init; } = Math.Clamp(Size, 1, 100);
}