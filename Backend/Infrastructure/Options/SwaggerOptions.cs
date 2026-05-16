namespace Infrastructure.Options;

public class SwaggerOptions
{
    public const string SectionName = "Swagger";
    public string Title { get; set; } = "Learn DotNet App";
    public string ApiDocs { get; set; } = "v1";
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = "API documentation for Learn DotNet App";
    public string ServerUrl { get; set; } = "http://localhost:9999";
    public string ContactName { get; set; } = "Le Duy Nhan";
    public string ContactEmail { get; set; } = "ldnhan.dev@gmail.com";
}
