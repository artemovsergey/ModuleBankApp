using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Models;
using Polly;

namespace ModuleBankApp.API.Infrastructure.Messaging.Producers;

public class CreateAccountProducer(IEventBusService eventBus,
                                   ILogger<CreateAccountProducer> logger,
                                   IServiceScopeFactory scopedFactory) : BackgroundService
{
    private static readonly Random Random = new Random();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopedFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
            
            var publishEvents = await db.Outbox
                .Where(e => e.Status == OutboxStatus.Pending && e.Type == nameof(AccountOpened))
                .ToListAsync(cancellationToken: stoppingToken);

            foreach (var @event in publishEvents)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    await Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(
                            retryCount: 5,
                            sleepDurationProvider: attempt =>
                            {
                                var exponential = Math.Pow(2, attempt); // 2, 4, 8, 16, 32 сек
                                var jitter = Random.NextDouble();       // 0..1 сек случайно
                                return TimeSpan.FromSeconds(exponential + jitter);
                            },
                            onRetry: (ex, ts, attempt, _) =>
                            {
                                logger.LogWarning(ex,
                                    "\n Ошибка при публикации события {@LogContext} \n",
                                    new
                                    {
                                        EventId = @event.Id,
                                        Type = @event.Type,
                                        Retry = attempt,
                                        Delay = ts
                                    });
                            })
                        .ExecuteAsync(() => eventBus.PublishAsync(@event, "account.opened", stoppingToken));

                    stopwatch.Stop();

                    logger.LogInformation("\n Событие опубликовано {@LogContext} \n",
                        new
                        {
                            EventId = @event.Id,
                            Type = @event.Type,
                            LatencyMs = stopwatch.ElapsedMilliseconds
                        });

                    @event.Status = OutboxStatus.Published;
                    db.Entry(@event).State = EntityState.Modified;
                    await db.SaveChangesAsync(stoppingToken); 
                    // TODO Update with batch 
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    
                    @event.Status = OutboxStatus.Failed;
                    db.Entry(@event).State = EntityState.Modified;
                    await db.SaveChangesAsync(stoppingToken);

                    logger.LogError(ex, "\n Не удалось опубликовать событие {@LogContext} \n",
                        new
                        {
                            EventId = @event.Id,
                            Type = @event.Type,
                            LatencyMs = stopwatch.ElapsedMilliseconds
                        });

                    continue;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}


/*

- Задержка Task.Delay(20s) фиксированная → это значит,
что при большом потоке событий будут задержки. 
Лучше использовать polling interval на основе нагрузки или механизм CDC (Change Data Capture).

- При переводе в Failed событие фактически теряется
(нет ретраев в будущем). Лучше сделать стратегию "Dead Letter" или "Requeue".

*/