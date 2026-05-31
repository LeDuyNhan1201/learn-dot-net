using BuildingBlocks.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Authentication.Extensions;

internal static class JwtBearerExtensions
{
    internal static void ConfigureJwtBearer(this JwtBearerOptions options)
    {
        // options.TokenValidationParameters = new TokenValidationParameters
        // {
        //     ValidateIssuer = true,
        //     ValidateAudience = true,
        //     ValidateLifetime = true,
        //     ValidateIssuerSigningKey = true,
        //     ValidIssuer = jwtOptions.Issuer,
        //     ValidAudiences = jwtOptions.Audiences,
        // };
        //
        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger =
                    context.HttpContext.RequestServices
                        .GetRequiredService<
                            ILoggerFactory>()
                        .CreateLogger("JwtBearer");

                logger.LogWarning(
                    context.Exception,
                    "JWT authentication failed");

                return Task.CompletedTask;
            },

            OnChallenge = async context =>
            {
                context.HandleResponse();

                if (context.Response.HasStarted) return;

                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                    new ErrorResponse(
                        "TOKEN_INVALID",
                        "Authentication required"));
            },

            OnForbidden = async context =>
            {
                if (context.Response.HasStarted) return;

                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                    new ErrorResponse(
                        "FORBIDDEN",
                        "You do not have permission"));
            }
        };
    }
}