using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Endpoints;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos").WithTags("Todo APIs");

        group.MapGet("/", (ITodoService service) => service.GetAll());

        group.MapGet("/{id:int}",
            Results<Ok<Todo>, NotFound> (int id, ITodoService service) =>
            {
                var todo = service.GetById(id);
                return todo is not null
                    ? TypedResults.Ok(todo)
                    : TypedResults.NotFound();
            });
        
        group.MapPost("/upload", (IFormFile file) => Results.Ok())
            .RequireAuthorization();

        return group;
    }
}