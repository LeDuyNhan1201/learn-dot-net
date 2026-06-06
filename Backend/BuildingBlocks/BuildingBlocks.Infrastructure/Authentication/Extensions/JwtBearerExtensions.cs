using BuildingBlocks.Infrastructure.Authentication.Serialization;
using BuildingBlocks.Shared.DTOs;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Authentication.Extensions;

internal static class JwtBearerExtensions
{
    internal static void ConfigureJwtBearer(this JwtBearerOptions options)
    {
        options.TokenValidationParameters.NameClaimType = KeycloakConstants.NameClaimType;
        options.TokenValidationParameters.RoleClaimType = KeycloakConstants.RoleClaimType;
        options.MapInboundClaims = false;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context
                    .HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger($"{nameof(JwtBearerExtensions)}.OnMessageReceived");

                logger.LogDebug(
                    "Authority: {Authority}, MetadataAddress: {MetadataAddress}, RequireHttpsMetadata: {RequireHttpsMetadata}",
                    options.Authority,
                    options.MetadataAddress,
                    options.RequireHttpsMetadata);

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var logger = context
                    .HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger($"{nameof(JwtBearerExtensions)}.OnAuthenticationFailed");

                logger.LogWarning(context.Exception, "JWT authentication failed");

                return Task.CompletedTask;
            },

            OnChallenge = async context =>
            {
                context.HandleResponse();

                if (context.Response.HasStarted) return;

                await context.Response.WriteAuthenticationErrorAsync(
                    StatusCodes.Status401Unauthorized,
                    new BaseResponse<object>(
                        "TOKEN_INVALID",
                        "Authentication required"),
                    context.HttpContext.RequestAborted);
            },

            OnForbidden = async context =>
            {
                if (context.Response.HasStarted) return;

                await context.Response.WriteAuthenticationErrorAsync(
                    StatusCodes.Status403Forbidden,
                    new BaseResponse<object>(
                        "FORBIDDEN",
                        "You do not have permission"),
                    context.HttpContext.RequestAborted);
            }
        };
    }

    private static Task WriteAuthenticationErrorAsync(
        this HttpResponse response,
        int statusCode,
        BaseResponse<object> baseResponse,
        CancellationToken cancellationToken)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";

        return response.WriteAsJsonAsync(
            baseResponse,
            AuthenticationJsonSerializerContext.Default.BaseResponseObject,
            "application/json",
            cancellationToken);
    }
}