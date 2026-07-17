namespace BuildingBlocks.Identity.keycloakAdmin.Options;

public sealed class KeycloakAdminOptions
{
    public const string Section = "KeycloakAdmin";

    public string Realm { get; init; } = string.Empty;

    public string AuthServerUrl { get; init; } = string.Empty;

    public string SslRequired { get; init; } = string.Empty;

    public string Resource { get; init; } = string.Empty;

    public CredentialsOptions Credentials { get; init; } = new();

    public int ConfidentialPort { get; init; }

    public sealed class CredentialsOptions
    {
        public string Secret { get; init; } = string.Empty;
    }
}