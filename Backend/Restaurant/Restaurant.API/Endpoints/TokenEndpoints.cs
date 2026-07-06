using System.Text.Json.Serialization;

namespace Restaurant.API.Endpoints;

public static class TokenEndpoints
{
    public static IEndpointRouteBuilder MapTokenEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/sessions", (TokenRequest request) =>
                Results.Ok(new TokenResponse(
                    "mock-access-token",
                    "mock-refresh-token")))
            .WithTags("Token APIs");

        return app;
    }
}

public sealed record TokenRequest(string Username, string Password);

public sealed record TokenResponse(
    [property: JsonPropertyName("access-token")]
    string AccessToken,
    [property: JsonPropertyName("refresh-token")]
    string RefreshToken);