using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Options;
using RabbitMQ.Client;

namespace ModuleBankApp.API.Services;

public class RabbitMqEventBus : IEventBus
{
    private readonly IRabbitMqConnectionService _connection;
    private readonly RabbitMqOptions _options;

    public RabbitMqEventBus(IRabbitMqConnectionService connection, IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        using var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Fanout, durable: true);

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json).AsMemory();

        await channel.BasicPublishAsync(_options.ExchangeName, "", body, cancellationToken: cancellationToken);
    }
}