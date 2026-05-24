using Application.Interfaces;
using BuildingBlocks.API.Interfaces;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Endpoints;

public class TodoEndpointsV1 : IEndpointModule
{
    public string Version => Version1DocumentTransfomer.Version1;
    
    public void MapEndpoints(IEndpointRouteBuilder app)
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
        
        group.MapPost("/upload", (IFormFile file) => Results.Ok()).RequireAuthorization();
    }
}