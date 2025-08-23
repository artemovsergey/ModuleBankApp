using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Messaging.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ModuleBankApp.API.Infrastructure.Messaging.Consumers;

public class CreateAccountConsumer(
    IServiceScopeFactory scopeFactory,
    IEventBusConnection connection,
    ILogger<CreateAccountConsumer> log)
    : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await connection.CreateChannelAsync();
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            EventEnvelope<OutboxMessage>? @envelope = null;

            try
            {
                @envelope = JsonSerializer.Deserialize<EventEnvelope<OutboxMessage>>(message);
                
                if (envelope is null)
                {
                    log.LogWarning("Не удалось десериализовать сообщение: {Message}", message);
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }
                
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
                
                // Проверка идемпотентности
                var alreadyProcessed = await db.Inbox.AnyAsync(m => m.Id == @envelope.Payload.Id, stoppingToken);
                
                if (alreadyProcessed)
                {
                    log.LogInformation("Сообщение {EventId} уже обработано", @envelope.Payload.Id);
                }
                else
                {
                    db.Inbox.Add(new InboxMessage
                    {
                        Id = @envelope.Payload.Id,
                        Type = @envelope.Payload.Type,
                        Payload = message,
                        ReceivedAtUtc = DateTimeOffset.UtcNow,
                        Processed = true
                    });

                    // TODO: бизнес-логика обработки
                    log.LogInformation("\n Обрабатываю новое сообщение {EventId} \n",@envelope.Payload.Id);

                    await db.SaveChangesAsync(stoppingToken);
                }
                
                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "\n Ошибка при обработке сообщения {EventId} \n", @envelope!.Payload.Id);

                // Сообщение вернётся в очередь
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, cancellationToken: stoppingToken);
            }
        };
        
        await channel.BasicQosAsync(0, 10, false, stoppingToken);
        await channel.BasicConsumeAsync(
            queue: "account.crm",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        log.LogInformation("\n Inbox consumer запущен для очереди account.opened \n");
    }
}

/*
   Аргумент ea (BasicDeliverEventArgs) содержит тело, заголовки, DeliveryTag и прочие метаданные.
 
    В UI ты видишь только "невыданные" сообщения. 
    Если consumer онлайн, очередь будет пустой — это нормально.
    Очередь — это временное буферное место, нормальная ситуация
    для event-driven архитектуры — очередь почти всегда пустая.
    
    QoS: стоит ограничить prefetch (например channel.BasicQos(0, 10, false)), чтобы consumer не загружался тысячами сообщений сразу.
*/