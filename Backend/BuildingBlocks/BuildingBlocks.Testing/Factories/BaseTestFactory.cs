using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BuildingBlocks.Testing.Factories;

public interface IBaseTestFactory : IAsyncDisposable
{
    HttpClient CreateClient();
}

public abstract class BaseTestFactory<TProgram> : WebApplicationFactory<TProgram>, IBaseTestFactory where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}