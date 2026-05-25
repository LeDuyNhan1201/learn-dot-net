using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Interfaces;

public interface IOptionsModule
{
    void Register(
        IServiceCollection services,
        IConfiguration configuration);
}