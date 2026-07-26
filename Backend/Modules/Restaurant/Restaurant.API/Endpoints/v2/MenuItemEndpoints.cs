using BuildingBlocks.API.Interfaces;
using BuildingBlocks.OpenApi.Versions;
using BuildingBlocks.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Domain.Contracts;

namespace Restaurant.API.Endpoints.v2;

public class MenuItemEndpoints : IEndpointModule
{
    public string Version => Version2DocumentTransformer.Version2;

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/menu-items").WithTags("Menu items APIs");

        group.MapPost("/", async (
            CreateMenuItemRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command =
                new CreateMenuItemCommand
                {
                    MenuItemName = request.MenuItemName!,
                    MenuItemDescription = request.MenuItemDescription!,
                    ImageUrl = request.MenuItemImageUrl!,
                    MenuItemPrice = request.MenuItemPrice,
                    Category = request.Category,
                    SubCategory = request.SubCategory
                };

            var id = await sender.Send(command, cancellationToken);

            return Results.Created($"/menu-items/{id}", new BaseResponse<string> { Data = id });
        });
    }
}