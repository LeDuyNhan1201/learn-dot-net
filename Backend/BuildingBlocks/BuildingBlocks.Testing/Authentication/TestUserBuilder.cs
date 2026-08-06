using System.Security.Claims;

namespace BuildingBlocks.Testing.Authentication;

public sealed class TestUserBuilder
{
    private readonly List<Claim> _claims = [];

    private readonly List<string> _permissions = [];

    private readonly List<string> _roles = [];

    private string? _email;

    private string? _tenant;
    private string _userId = Guid.NewGuid().ToString();

    private string _userName = "integration-user";

    public TestUserBuilder WithUserId(string userId)
    {
        _userId = userId;
        return this;
    }

    public TestUserBuilder WithUserName(string userName)
    {
        _userName = userName;
        return this;
    }

    public TestUserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public TestUserBuilder WithTenant(string tenant)
    {
        _tenant = tenant;
        return this;
    }

    public TestUserBuilder WithRole(string role)
    {
        _roles.Add(role);
        return this;
    }

    public TestUserBuilder WithPermission(string permission)
    {
        _permissions.Add(permission);
        return this;
    }

    public TestUserBuilder WithClaim(Claim claim)
    {
        _claims.Add(claim);
        return this;
    }

    internal IReadOnlyCollection<Claim> Build()
    {
        var claims = new List<Claim>
        {
            new(TestClaims.UserId, _userId),
            new(TestClaims.UserName, _userName)
        };

        if (!string.IsNullOrWhiteSpace(_email)) claims.Add(new Claim(TestClaims.Email, _email));
        if (!string.IsNullOrWhiteSpace(_tenant)) claims.Add(new Claim(TestClaims.Tenant, _tenant));

        claims.AddRange(_roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(_permissions.Select(permission => new Claim(TestClaims.Permission, permission)));
        claims.AddRange(_claims);

        return claims;
    }
}