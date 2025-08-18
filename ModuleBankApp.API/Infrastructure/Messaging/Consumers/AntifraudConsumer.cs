using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ModuleBankApp.API.Infrastructure.Messaging.Consumers;

public class AntifraudConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConnectionService _connection;
    private readonly RabbitMqOptions _options;

    public AntifraudConsumer(IServiceScopeFactory scopeFactory, IRabbitMqConnectionService connection,
        IOptions<RabbitMqOptions> options)
    {
        _scopeFactory = scopeFactory;
        _connection = connection;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Topic, durable: true);
        await channel.QueueDeclareAsync("account.antifraud", durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync("account.antifraud", _options.ExchangeName, "");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var evt = JsonSerializer.Deserialize<AntifraudEvent>(message);

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            if (evt?.Action == "client.blocked")
                await repo.FreezeAccount(evt.ClientId, true);
            else if (evt?.Action == "client.unblocked")
                await repo.FreezeAccount(evt.ClientId, false);

            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync("account.antifraud", false, consumer);
    }
}

public class AntifraudEvent
{
    public Guid ClientId { get; set; }
    public string Action { get; set; } = string.Empty;
}