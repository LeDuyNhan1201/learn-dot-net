using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services.Interfaces;

public interface ITodoService
{
    IReadOnlyList<Todo> GetAll();

    Todo? GetById(int id);
}