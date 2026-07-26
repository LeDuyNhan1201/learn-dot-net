using System.Diagnostics;
using BuildingBlocks.API.Interfaces;
using BuildingBlocks.API.Validation;
using BuildingBlocks.OpenApi.Versions;
using Restaurant.Application.DTOs;

namespace Restaurant.API.Endpoints.v1;

public class MenuItemEndpoints : IEndpointModule
{
    public string Version => Version1DocumentTransformer.Version1;

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/menu-items").WithTags("Menu items APIs");

        group
            .MapPost("", (CreateMenuItemRequest request) =>
            {
                Debugger.Break();
                return Results.Ok();
            })
            .AddEndpointFilter<ValidationFilter<CreateMenuItemRequest, Messages>>();
    }
}