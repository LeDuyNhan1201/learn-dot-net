using System.Reflection;
using BuildingBlocks.Domain.Contracts;
using BuildingBlocks.Messaging.Configurations;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.SharedKernel.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Extensions;

public static class EventExtensions
{
    public static string GetEventName(this Type type)
    {
        return type.GetCustomAttribute<EventNameAttribute>()?.Name ?? type.Name;
    }
    
    public static IServiceCollection AddMessaging<T>(
        this IServiceCollection services, 
        params Type[] consumerTypes)
    where T : DbContext
    {
        services.ConfigureInMemory<T>(consumerTypes);
        services.AddScoped<IDomainEventExecutor, DomainEventExecutor>();
        return services;
    }
}