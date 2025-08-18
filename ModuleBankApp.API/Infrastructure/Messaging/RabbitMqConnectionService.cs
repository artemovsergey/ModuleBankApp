    using Microsoft.Extensions.Options;
    using ModuleBankApp.API.Infrastructure.Messaging.Options;
    using RabbitMQ.Client;

    namespace ModuleBankApp.API.Infrastructure.Messaging;

    public interface IRabbitMqConnectionService : IDisposable
    {
        Task<IChannel> CreateChannelAsync();
    }

    public class RabbitMqConnectionService : IRabbitMqConnectionService, IDisposable
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

            // одно соединение на всё приложение
            //_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
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
            
            // каждый вызов возвращает новый канал
            var channel = factory.CreateConnectionAsync().GetAwaiter().GetResult().CreateChannelAsync();
            return await channel;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

