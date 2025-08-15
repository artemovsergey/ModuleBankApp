using System.Text;
using System.Text.Json;
using ModuleBankApp.API.Data.Interfaces;
using RabbitMQ.Client;

namespace ModuleBankApp.API.Services;

public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private RabbitMqEventBus(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqEventBus> CreateAsync(
        string user, string pass, string vhost, string hostName)
    {
        var factory = new ConnectionFactory
        {
            UserName = user,
            Password = pass,
            VirtualHost = vhost,
            HostName = hostName
        };

        var conn = await factory.CreateConnectionAsync();
        var channel = await conn.CreateChannelAsync();
        
        await channel.ExchangeDeclareAsync("accounts", ExchangeType.Fanout, durable: true);
        await channel.QueueDeclareAsync(
            queue: "accounts_queue",
            durable: true,       // очередь выживает после рестарта брокера
            exclusive: false,    // очередь может использоваться другими подключениями
            autoDelete: false,   // очередь не удаляется автоматически
            arguments: null
        );
        await channel.QueueBindAsync("accounts_queue", "accounts", "");
        
        return new RabbitMqEventBus(conn, channel);
    }
    
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        var exchange = "accounts";
        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json).AsMemory();

        await _channel.BasicPublishAsync(exchange,"", body, cancellationToken: cancellationToken);
        
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}