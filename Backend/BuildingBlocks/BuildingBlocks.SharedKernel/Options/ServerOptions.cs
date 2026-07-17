namespace BuildingBlocks.SharedKernel.Options;

public sealed class ServerOptions
{
    public const string Section = "Server";
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? BasePath { get; set; }
}