using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging;
using ModuleBankApp.API.Infrastructure.Messaging.Consumers;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using ModuleBankApp.API.Infrastructure.Messaging.Producers;

namespace ModuleBankApp.API.Extensions;

public static class EventBusServices
{
    public static IServiceCollection AddEventBusServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<EventBusOptions>(config.GetSection("RabbitMq"));
        
        services.AddSingleton<IEventBusConnectionService, EventBusConnectionService>();
        services.AddSingleton<IEventBus, EventBus>();
        
        services.AddHostedService<AntifraudConsumer>();
        services.AddHostedService<CreateAccountProducer>();
        services.AddHostedService<CreateAccountConsumer>();
        services.AddHostedService<AuditConsumer>();
        
        return services;
    }
}