using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ModuleBankApp.API.Infrastructure.Messaging.Consumers;

public class AntifraudConsumer(
    IServiceScopeFactory scopeFactory,
    IEventBusConnection connection,
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
     
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
            
            switch (ea.RoutingKey)
            {
                case "client.blocked":
                    var blocked = JsonSerializer.Deserialize<ClientBlocked>(message);
                    await repo.FreezeAccount(blocked!.ClientId, true);
                    break;

                case "client.unblocked":
                    var unblocked = JsonSerializer.Deserialize<ClientUnblocked>(message);
                    await repo.FreezeAccount(unblocked!.ClientId, false);
                    break;
            }
            
            await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
        };

        await channel.BasicConsumeAsync("account.antifraud", false, consumer, cancellationToken: stoppingToken);
    }
}

public class AntifraudEvent
{
    public Guid ClientId { get; set; }
    public string Action { get; set; } = string.Empty;
}