namespace Infrastructure.Options;

public sealed class ServerOptions
{
    public const string SectionName = "Server";
    public string? Name { get; set; }
    public string Version { get; set; } = "1.0.0";
    public string ApiVersion { get; set; } = "v1";
    
    public string BasePath { get; set; } = "api";
}