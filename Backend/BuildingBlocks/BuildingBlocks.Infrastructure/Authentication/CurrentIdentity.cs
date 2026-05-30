using BuildingBlocks.Infrastructure.Authentication;

namespace BuildingBlocks.API.Middlewares;

public sealed class CurrentIdentity
{
    public IdentityContext? Identity { get; set; }
}