using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging;
using ModuleBankApp.API.Infrastructure.Messaging.Consumers;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using ModuleBankApp.API.Infrastructure.Messaging.Producers;

namespace ModuleBankApp.API.Extensions;

public static class RabbitMqServices
{
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration config)
    {
        // Регистрация опций RabbitMQ
        services.Configure<RabbitMqOptions>(config.GetSection("RabbitMq"));
        
        services.AddSingleton<IRabbitMqConnectionService, RabbitMqConnectionService>();
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.AddHostedService<AntifraudConsumer>();
        services.AddHostedService<CreateAccountProducer>();
        return services;
    }
}