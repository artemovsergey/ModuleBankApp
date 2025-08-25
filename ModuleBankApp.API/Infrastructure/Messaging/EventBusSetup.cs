using RabbitMQ.Client;

namespace ModuleBankApp.API.Infrastructure.Messaging;

public static class EventBusSetup
{
    private const String ExchangeName = "account.events";
    public static async Task SetupQueuesAsync(IEventBusConnection connection,
                                              CancellationToken ct = default)
    {
        await using var channel = await connection.CreateChannelAsync();
      
        // 1. Создаём Topic Exchange
        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);
    
        // 2. Очередь account.crm (ловит account.*)
        await channel.QueueDeclareAsync(
            queue: "account.crm",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);
    
        await channel.QueueBindAsync(
            queue: "account.crm",
            exchange: ExchangeName,
            routingKey: "account.*",
            cancellationToken: ct);
    
        // 3. Очередь account.notifications (ловит money.*)
        await channel.QueueDeclareAsync(
            queue: "account.notifications",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);
    
        await channel.QueueBindAsync(
            queue: "account.notifications",
            exchange: ExchangeName,
            routingKey: "money.*",
            cancellationToken: ct);
    
        // 4. Очередь account.antifraud (ловит client.*)
        await channel.QueueDeclareAsync(
            queue: "account.antifraud",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);
    
        await channel.QueueBindAsync(
            queue: "account.antifraud",
            exchange: ExchangeName,
            routingKey: "client.*",
            cancellationToken: ct);
    
        // 5. Очередь account.audit (ловит всё: #)
        await channel.QueueDeclareAsync(
            queue: "account.audit",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);
    
        await channel.QueueBindAsync(
            queue: "account.audit",
            exchange: ExchangeName,
            routingKey: "#",
            cancellationToken: ct);
        
    }
}
