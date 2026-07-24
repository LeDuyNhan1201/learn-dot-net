using BuildingBlocks.Domain.Contracts;

namespace BuildingBlocks.Domain.Events;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _events = [];
    public IReadOnlyCollection<IDomainEvent> Events => _events;

    protected void Raise(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public IReadOnlyCollection<IDomainEvent> DequeueEvents()
    {
        var events = _events.ToArray();
        _events.Clear();
        return events;
    }
}