using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Messaging.Inbox;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using ModuleBankApp.API.Infrastructure.Messaging.Outbox;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ModuleBankApp.API.Infrastructure.Messaging.Consumers;

public class CreateAccountConsumer(
    IServiceScopeFactory scopeFactory,
    IEventBusConnectionService connection,
    ILogger<CreateAccountConsumer> log,
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
            OutboxMessage? @event = null;

            try
            {
                @event = JsonSerializer.Deserialize<OutboxMessage>(message);
                log.LogInformation("@event.Id = ",@event.Id);
                log.LogInformation("@event.Payload = ",@event.Payload);
                log.LogInformation("@event.Type = ",@event.Type);
                
                if (@event is null)
                {
                    log.LogWarning("Не удалось десериализовать сообщение: {Message}", message);
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }
                
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
                
                // Проверка идемпотентности
                var alreadyProcessed = await db.Inbox.AnyAsync(i => i.Id == @event.Id, stoppingToken);
                
                if (alreadyProcessed)
                {
                    log.LogInformation("Сообщение {EventId} уже обработано", @event.Id);
                }
                else
                {
                    // сохраняем в Inbox
                    db.Inbox.Add(new InboxMessage
                    {
                        Id = @event.Id,
                        
                        Payload = message,
                        ReceivedAtUtc = DateTimeOffset.UtcNow,
                        Processed = true // сразу пометим обработанным
                    });

                    // TODO: бизнес-логика обработки
                    log.LogInformation("Обрабатываю новое сообщение {EventId}",@event.Id);

                    await db.SaveChangesAsync(stoppingToken);
                }
                
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Ошибка при обработке сообщения {EventId}", @event?.Id);

                // Сообщение вернётся в очередь
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: "account.opened",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
        
        log.LogInformation("Inbox consumer запущен для очереди account.opened");
    }
}

/*
    В UI ты видишь только "невыданные" сообщения. 
    Если consumer онлайн, очередь будет пустой — это нормально.
    Очередь — это временное буферное место, нормальная ситуация
    для event-driven архитектуры — очередь почти всегда пустая.
*/