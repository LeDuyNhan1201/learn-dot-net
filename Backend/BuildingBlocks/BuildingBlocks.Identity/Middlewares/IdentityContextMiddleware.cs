using BuildingBlocks.Domain.Enumerations;
using BuildingBlocks.Domain.ExecutionContext;
using BuildingBlocks.Domain.ExecutionContext.Interfaces;
using BuildingBlocks.Identity.Extensions;
using BuildingBlocks.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Identity.Middlewares;

public sealed class IdentityContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IExecutionContextInitializer initializer,
        IExecutionContextAccessor accessor)
    {
        // Bypass the middleware if the endpoint does not require authorization
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAuthorizeData>() is null)
        {
            await next(context);
            return;
        }

        initializer.Initialize(new AppExecutionContext
        {
            UserId = context.User.GetUserId(),

            Username = context.User.GetUsername(),

            Email = context.User.GetEmail(),

            ActorType = context.User.Identity?.IsAuthenticated == true
                ? ActorType.User
                : ActorType.System,

            CorrelationId = context.TraceIdentifier,

            RequestId = context.TraceIdentifier,

            ClientIp = context.Connection.RemoteIpAddress?.ToString(),

            UserAgent = context.Request.Headers.UserAgent.ToString(),

            Source = "HTTP"
        });

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

        try
        {
            await next(context);
        }
        finally
        {
            accessor.Clear();
        }
    }
}