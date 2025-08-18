using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using RabbitMQ.Client;

namespace ModuleBankApp.API.Infrastructure.Messaging;

public class RabbitMqEventBus(IRabbitMqConnectionService connection, IOptions<RabbitMqOptions> options)
    : IEventBus
{
    private readonly RabbitMqOptions _options = options.Value;

    // отправляет сообщение в очередь (exchange) в Rabbit
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        await using var channel = await connection.CreateChannelAsync();

        // //Exchange
        // await channel.ExchangeDeclareAsync(
        //     _options.ExchangeName,
        //     ExchangeType.Topic,
        //     durable: true,
        //     cancellationToken: cancellationToken);
        //
        // //Create queue and bind to exchange
        // var queueName = $"account.opened";
        // await channel.QueueDeclareAsync(
        //     queue: queueName,
        //     durable: true,
        //     exclusive: false,
        //     autoDelete: false,
        //     cancellationToken: cancellationToken);
        //
        // await channel.QueueBindAsync(
        //     queue: queueName,
        //     exchange: _options.ExchangeName,
        //     routingKey: "opened",
        //     cancellationToken: cancellationToken);
        
        // Envelope
        var envelope = new
        {
            eventId = Guid.NewGuid(),
            occurredAt = DateTime.UtcNow.ToString("o"), // ISO-8601
            payload = @event,
            meta = new
            {
                version = "v1",
                source = "account-service", // или из config
                correlationId = (@event as IHasCorrelation)?.CorrelationId ?? Guid.NewGuid(),
                causationId = (@event as IHasCausation)?.CausationId ?? Guid.NewGuid()
            }
        };
        
        // Properties
        var props = new BasicProperties();
        props.Persistent = true;
        props.Headers = new Dictionary<string, object>
        {
            { "X-Correlation-Id", envelope.meta.correlationId.ToString() },
            { "X-Causation-Id", envelope.meta.causationId.ToString() },
            { "Source", envelope.meta.source },
            { "Version", envelope.meta.version }
        }!;
        
        // Serialize
        var json = JsonSerializer.Serialize(envelope);
        var body = Encoding.UTF8.GetBytes(json).AsMemory();
        
       
        // Publish
        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: "opened",
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }

}

public interface IHasCorrelation
{
    Guid CorrelationId { get; }
}

public interface IHasCausation
{
    Guid CausationId { get; }
}