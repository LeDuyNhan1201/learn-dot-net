using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Interfaces;

public interface IInfrastructureModule
{
    void Register(
        IServiceCollection services,
        IConfiguration configuration);
}