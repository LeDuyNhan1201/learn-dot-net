using BuildingBlocks.API.Interfaces;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.OpenApi.Versions;
using BuildingBlocks.SharedKernel.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.API.Endpoints;

public class DataInitializationEndpoints : IEndpointModule
{
    public string Version => Version1DocumentTransformer.Version1;

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/init").WithTags("Data initialization APIs");

        group.MapPost("/users", async ([FromServices] IUserSeederService service, CancellationToken cancellationToken) =>
        {
            var adminIds = await service.InitAdministrators(cancellationToken);
            var customerIds = await service.InitCustomers(cancellationToken);
            var userIds = adminIds.Concat(customerIds).ToList();
            return Results.Created($"/users", new BaseResponse<IReadOnlyCollection<string>> { Data = userIds });
        });
    }
}