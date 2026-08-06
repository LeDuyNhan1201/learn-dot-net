using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Testing.Authentication;

public static class TestJwtGenerator
{
    private static readonly TestJwtOptions Options = new();

    public static string Create(
        TestUserBuilder? builder = null,
        DateTime? expires = null,
        string? issuer = null,
        string? audience = null,
        string? secret = null)
    {
        builder ??= new TestUserBuilder();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? Options.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer ?? Options.Issuer,
            audience ?? Options.Audience,
            builder.Build(),
            DateTime.UtcNow,
            expires ?? DateTime.UtcNow.Add(Options.Lifetime),
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string Expired()
    {
        return Create(expires: DateTime.UtcNow.AddMinutes(-5));
    }

    public static string InvalidSignature()
    {
        return Create(secret: "another-secret-key-12345678901234567890");
    }

    public static string WrongIssuer()
    {
        return Create(issuer: "another-issuer");
    }

    public static string WrongAudience()
    {
        return Create(audience: "another-audience");
    }

    public static string Admin()
    {
        return Create(new TestUserBuilder().WithRole("Admin"));
    }

    public static string User()
    {
        return Create(new TestUserBuilder().WithRole("User"));
    }
}