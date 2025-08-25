    using Microsoft.Extensions.Options;
    using ModuleBankApp.API.Infrastructure.Messaging.Options;
    using RabbitMQ.Client;

    namespace ModuleBankApp.API.Infrastructure.Messaging;

    public interface IEventBusConnection : IDisposable
    {
        Task<IChannel> CreateChannelAsync();
    }

    public class EventBusConnection : IEventBusConnection
    {
        private readonly IConnection _connection;
        public EventBusConnection(IOptions<EventBusOptions> options)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                VirtualHost = options.Value.VirtualHost
            };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            var channel = await _connection.CreateChannelAsync();
            return channel;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

