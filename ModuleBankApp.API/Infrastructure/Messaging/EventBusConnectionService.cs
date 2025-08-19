    using Microsoft.Extensions.Options;
    using ModuleBankApp.API.Infrastructure.Messaging.Options;
    using RabbitMQ.Client;

    namespace ModuleBankApp.API.Infrastructure.Messaging;

    public interface IEventBusConnectionService : IDisposable
    {
        Task<IChannel> CreateChannelAsync();
    }

    public class EventBusConnectionService : IEventBusConnectionService
    {
        private readonly IConnection _connection;
        public EventBusConnectionService(IOptions<EventBusOptions> options)
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
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            
            var channel = factory.CreateConnectionAsync().GetAwaiter().GetResult().CreateChannelAsync();
            return await channel;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

