using Domain.Entities;

namespace Application.Interfaces;

public interface ITodoService
{
    IReadOnlyList<Todo> GetAll();

    Todo? GetById(int id);
}