namespace BuildingBlocks.Shared.Options;

public sealed class ServerOptions
{
    public const string SectionName = "Server";
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? BasePath { get; set; }
}