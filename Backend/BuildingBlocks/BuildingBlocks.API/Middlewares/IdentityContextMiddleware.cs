using BuildingBlocks.Infrastructure.Authentication.Extensions;
using BuildingBlocks.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Middlewares;

public sealed class IdentityContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var currentIdentity = context.RequestServices.GetRequiredService<CurrentIdentity>();

            currentIdentity.Identity =
                new IdentityContext
                {
                    UserId = context.User.GetUserId(),

                    Username = context.User.GetUsername(),

                    Email = context.User.GetEmail()
                };
        }

        await next(context);
    }
}