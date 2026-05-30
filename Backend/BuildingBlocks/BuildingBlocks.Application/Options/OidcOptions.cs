namespace BuildingBlocks.Application.Options;

public sealed class OidcOptions
{
    public const string SectionName = "OIDC";
    public string[] ServerUrls { get; set; } = [];
    public string? ApiDocs { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
}