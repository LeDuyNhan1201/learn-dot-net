using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Testing.Authentication;

public static class TestAuthenticationExtensions
{
    public static IServiceCollection ConfigureTestJwtAuthentication(this IServiceCollection services)
    {
        services.RemoveAll<IConfigureOptions<JwtBearerOptions>>();
        services.RemoveAll<IPostConfigureOptions<JwtBearerOptions>>();

        var options = new TestJwtOptions();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = options.SecurityKey,
                    NameClaimType = TestClaims.UserName,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        return services;
    }
}