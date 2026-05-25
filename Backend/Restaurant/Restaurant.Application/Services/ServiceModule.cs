using Application.Interfaces;
using BuildingBlocks.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class ServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
    }
}