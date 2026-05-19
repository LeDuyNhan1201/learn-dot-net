namespace Infrastructure.Options;

public sealed class OpenApiOptions
{
    public const string SectionName = "OpenApi";
    public string? Title { get; set; }
    public string? ApiDocs { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public string? ServerUrl { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
}
