using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public sealed class TodoService : ITodoService
{
    private static readonly Todo[] Todos =
    [
        new()
        {
            Id = 1,
            Title = "Walk the dog",
            DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
        },

        new()
        {
            Id = 2,
            Title = "Do the dishes",
            DueBy = DateOnly.FromDateTime(DateTime.Now)
        }
    ];

    public IReadOnlyList<Todo> GetAll()
    {
        return Todos;
    }

    public Todo? GetById(int id)
    {
        return Todos.FirstOrDefault(x => x.Id == id);
    }
}