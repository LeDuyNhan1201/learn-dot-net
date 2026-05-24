using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Interfaces;

public interface IServiceModule
{
    void Register(IServiceCollection services);
}