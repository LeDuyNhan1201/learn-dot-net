namespace Restaurant.Domain.Entities;

public sealed class Todo
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public DateOnly? DueBy { get; init; }

    public bool IsComplete { get; init; }
}