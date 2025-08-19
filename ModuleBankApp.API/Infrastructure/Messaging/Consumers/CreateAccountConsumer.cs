using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using ModuleBankApp.API.Infrastructure.Messaging.Outbox;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ModuleBankApp.API.Infrastructure.Messaging.Consumers;

public class CreateAccountConsumer(
    IServiceScopeFactory scopeFactory,
    IEventBusConnectionService connection,
    ILogger<CreateAccountConsumer> log,
    IOptions<EventBusOptions> options)
    : BackgroundService
{
    private readonly EventBusOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await connection.CreateChannelAsync();
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var evt = JsonSerializer.Deserialize<OutboxMessage>(message);
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
        };

        await channel.BasicConsumeAsync("account.opened", false, consumer);
        
    }
}

/*
    В UI ты видишь только "невыданные" сообщения. 
    Если consumer онлайн, очередь будет пустой — это нормально.
    Очередь — это временное буферное место, нормальная ситуация
    для event-driven архитектуры — очередь почти всегда пустая.
*/