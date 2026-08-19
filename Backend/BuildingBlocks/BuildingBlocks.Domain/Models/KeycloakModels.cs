using System.Text.Json.Serialization;
using BuildingBlocks.Domain.Enumerations;

namespace BuildingBlocks.Domain.Models;

public sealed record CreateKeycloakRoleRequest(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("description")]
    string? Description);

public sealed record CreateKeycloakUserRequest(
    [property: JsonPropertyName("username")]
    string Username,

    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("firstName")]
    string FirstName,

    [property: JsonPropertyName("lastName")]
    string LastName,

    [property: JsonPropertyName("password")]
    string Password,

    [property: JsonPropertyName("enabled")]
    bool Enabled,

    [property: JsonPropertyName("emailVerified")]
    bool EmailVerified,

    [property: JsonPropertyName("attributes")]
    IDictionary<string, ICollection<string>>? Attributes,
    
    [property: JsonPropertyName("group")]
    KeycloakUserGroup Group);

public sealed record KeycloakTokenResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken,

    [property: JsonPropertyName("expires_in")]
    int ExpiresIn,

    [property: JsonPropertyName("refresh_token")]
    string? RefreshToken,

    [property: JsonPropertyName("refresh_expires_in")]
    int? RefreshExpiresIn,

    [property: JsonPropertyName("token_type")]
    string TokenType);