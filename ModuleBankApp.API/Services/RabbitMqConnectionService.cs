using Microsoft.Extensions.Options;
using ModuleBankApp.API.Options;
using RabbitMQ.Client;

namespace ModuleBankApp.API.Services;

public interface IRabbitMqConnectionService : IDisposable
{
    Task<IChannel> CreateChannelAsync();
}

public class RabbitMqConnectionService : IRabbitMqConnectionService
{
    private readonly IConnection _connection;

    public RabbitMqConnectionService(IOptions<RabbitMqOptions> options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            VirtualHost = options.Value.VirtualHost
        };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        return await _connection.CreateChannelAsync();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
