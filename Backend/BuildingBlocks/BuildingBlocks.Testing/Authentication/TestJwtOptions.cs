using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Testing.Authentication;

public sealed class TestJwtOptions
{
    public string Issuer { get; init; } = TestAuthenticationDefaults.Issuer;
    public string Audience { get; init; } = TestAuthenticationDefaults.Audience;
    public string Secret { get; init; } = TestAuthenticationDefaults.Secret;
    public TimeSpan Lifetime { get; init; } = TimeSpan.FromHours(1);
    public SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes(Secret));
    public SigningCredentials SigningCredentials => new(SecurityKey, SecurityAlgorithms.HmacSha256);
}