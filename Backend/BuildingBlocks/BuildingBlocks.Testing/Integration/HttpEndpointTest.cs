using System.Net.Http.Json;
using Xunit;

namespace BuildingBlocks.Testing.Integration;

public abstract class HttpEndpointTest
{
    protected abstract HttpClient Client { get; }
    protected abstract IServiceProvider Services { get; }
    
    protected async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await Client.GetAsync(uri, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string uri, T request) where T : class
    {
        return await Client.PostAsJsonAsync(uri, request, TestContext.Current.CancellationToken);
    }
    
    protected async Task<HttpResponseMessage> PutAsync<T>(string uri, T request) where T : class
    {
        return await Client.PutAsJsonAsync(uri, request, TestContext.Current.CancellationToken);
    }
    
    protected async Task<HttpResponseMessage> PatchAsync<T>(string uri, T request) where T : class
    {
        return await Client.PatchAsJsonAsync(uri, request, TestContext.Current.CancellationToken);
    }
    
    protected async Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        return await Client.DeleteAsync(uri, TestContext.Current.CancellationToken);
    }
    
    protected async Task<HttpResponseMessage> PostMultipartAsync(
        string uri,
        IEnumerable<(Stream Stream, string FileName, string FieldName, string? ContentType)> files,
        IDictionary<string, string>? fields = null)
    {
        using var content = new MultipartFormDataContent();

        foreach (var file in files)
        {
            var fileContent = new StreamContent(file.Stream);

            if (!string.IsNullOrWhiteSpace(file.ContentType))
            {
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            }

            content.Add(fileContent, file.FieldName, file.FileName);
        }
        
        var response = await Client.PostAsync(uri, content, TestContext.Current.CancellationToken);
        if (fields is null) return response;
        
        foreach (var field in fields)
        {
            content.Add(new StringContent(field.Value), field.Key);
        }

        return response;
    }
}