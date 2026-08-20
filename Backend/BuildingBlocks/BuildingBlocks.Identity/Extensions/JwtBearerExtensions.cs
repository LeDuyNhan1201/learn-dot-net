using System.Text.Encodings.Web;
using System.Text.Json;
using BuildingBlocks.Identity.Serialization;
using BuildingBlocks.SharedKernel.DTOs;
using BuildingBlocks.SharedKernel.Errors.Models;
using BuildingBlocks.SharedKernel.Localization;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Identity.Extensions;

internal static class JwtBearerExtensions
{
    internal static void ConfigureJwtBearer<T>(this JwtBearerOptions options)
        where T : class
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = KeycloakConstants.NameClaimType,
            RoleClaimType = KeycloakConstants.RoleClaimType
        };
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
                var localizer = context
                    .HttpContext.RequestServices
                    .GetRequiredService<CompositeLocalizer<T>>();

                context.HandleResponse();

                if (context.Response.HasStarted) return;

                await context.Response.WriteAuthenticationErrorAsync(
                    StatusCodes.Status401Unauthorized,
                    new BaseResponse<object>
                    {
                        Code = AuthErrors.Unauthorized.Code,
                        Message = localizer[AuthErrors.Unauthorized.MessageKey]
                    },
                    context.HttpContext.RequestAborted);
            },

            OnForbidden = async context =>
            {
                var localizer = context
                    .HttpContext.RequestServices
                    .GetRequiredService<CompositeLocalizer<T>>();

                if (context.Response.HasStarted) return;

                await context.Response.WriteAuthenticationErrorAsync(
                    StatusCodes.Status403Forbidden,
                    new BaseResponse<object>
                    {
                        Code = AuthErrors.Forbidden.Code,
                        Message = localizer[AuthErrors.Forbidden.MessageKey]
                    },
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
        response.ContentType = "application/json; charset=utf-8";

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            TypeInfoResolver = AuthenticationJsonSerializerContext.Default
        };

        return response.WriteAsJsonAsync(
            baseResponse,
            options,
            response.ContentType,
            cancellationToken);
    }
}