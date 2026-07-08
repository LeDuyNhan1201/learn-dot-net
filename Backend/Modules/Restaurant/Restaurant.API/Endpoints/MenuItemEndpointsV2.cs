using BuildingBlocks.API.Interfaces;
using BuildingBlocks.Infrastructure.OpenApi.Versions;
using BuildingBlocks.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Contracts;
using Restaurant.Application.DTOs;

namespace Restaurant.API.Endpoints;

public class MenuItemEndpointsV2 : IEndpointModule
{
    public string Version => Version2DocumentTransformer.Version2;

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/menu-items").WithTags("Menu items APIs");

        group.MapPost("/", async (
            IMenuItemDto.CreateRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command =
                new IMenuItemCommand.Create
                {
                    MenuItemName = request.MenuItemName!,
                    MenuItemDescription = request.MenuItemDescription!,
                    ImageUrl = request.MenuItemImageUrl!,
                    MenuItemPrice = request.MenuItemPrice
                };

            var id = await sender.Send(command, cancellationToken);

            return Results.Created($"/menu-items/{id}", new BaseResponse<string> { Data = id });
        });
    }
}