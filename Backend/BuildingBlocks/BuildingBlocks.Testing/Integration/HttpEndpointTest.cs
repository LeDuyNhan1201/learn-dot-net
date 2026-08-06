using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BuildingBlocks.Testing.Integration;

public abstract class HttpEndpointTest
{
    protected abstract HttpClient Client { get; }
    protected abstract IServiceProvider Services { get; }

    private HttpRequestMessage CreateRequest(HttpMethod method, string uri, string? bearerToken = null)
    {
        var request = new HttpRequestMessage(method, uri);

        if (!string.IsNullOrWhiteSpace(bearerToken)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        return request;
    }

    protected async Task<HttpResponseMessage> GetAsync(
        string uri,
        string? bearerToken = null)
    {
        using var request = CreateRequest(HttpMethod.Get, uri, bearerToken);

        return await Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(
        string uri,
        T requestBody,
        string? bearerToken = null)
        where T : class
    {
        using var request = CreateRequest(HttpMethod.Post, uri, bearerToken);
        request.Content = JsonContent.Create(requestBody);

        return await Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(
        string uri,
        T requestBody,
        string? bearerToken = null)
        where T : class
    {
        using var request = CreateRequest(HttpMethod.Put, uri, bearerToken);
        request.Content = JsonContent.Create(requestBody);

        return await Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PatchAsync<T>(
        string uri,
        T requestBody,
        string? bearerToken = null)
        where T : class
    {
        using var request = CreateRequest(HttpMethod.Patch, uri, bearerToken);
        request.Content = JsonContent.Create(requestBody);

        return await Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(
        string uri,
        string? bearerToken = null)
    {
        using var request = CreateRequest(HttpMethod.Delete, uri, bearerToken);

        return await Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    protected async Task<HttpResponseMessage> PostMultipartAsync(
        string uri,
        IEnumerable<(Stream Stream, string FileName, string FieldName, string? ContentType)> files,
        IDictionary<string, string>? fields = null,
        string? bearerToken = null)
    {
        using var request = CreateRequest(HttpMethod.Post, uri, bearerToken);
        using var content = new MultipartFormDataContent();

        foreach (var file in files)
        {
            var fileContent = new StreamContent(file.Stream);

            if (!string.IsNullOrWhiteSpace(file.ContentType)) fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, file.FieldName, file.FileName);
        }

        if (fields is not null)
            foreach (var field in fields)
                content.Add(new StringContent(field.Value), field.Key);

        request.Content = content;

        return await Client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}