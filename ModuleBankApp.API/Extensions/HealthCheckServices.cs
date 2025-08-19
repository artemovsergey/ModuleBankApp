using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Messaging;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using ModuleBankApp.API.Infrastructure.Messaging.Outbox;
using ModuleBankApp.API.Services;
using RabbitMQ.Client;

namespace ModuleBankApp.API.Extensions;

public static class HealthCheckServices
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration config)
    {

        services.AddHealthChecks()
            // Проверка подключения к БД
            .AddNpgSql(config.GetConnectionString("PostgreSQL")!, name: "Postgres")

            // Проверка RabbitMQ
            .AddCheck<RabbitMqHealthCheck>("RabbitMQ")

            // Проверка Outbox
            .AddCheck<OutboxHealthCheck>("OutboxPending");

        return services;
    }
}

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IOptions<EventBusOptions> _options;
    private readonly ILogger<RabbitMqHealthCheck> _logger;

    public RabbitMqHealthCheck(IOptions<EventBusOptions> options, ILogger<RabbitMqHealthCheck> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rmqConnection = new EventBusConnectionService(_options);
            var rmqChannel = await rmqConnection.CreateChannelAsync();
            await rmqChannel.CloseAsync();

            _logger.LogInformation("HEALTH CHECK: RabbitMQ connection OK");
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HEALTH CHECK: RabbitMQ connection failed");
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}

public class OutboxHealthCheck : IHealthCheck
{
    private readonly ModuleBankAppContext _db;
    private readonly ILogger<OutboxHealthCheck> _logger;

    public OutboxHealthCheck(ModuleBankAppContext db, ILogger<OutboxHealthCheck> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var pendingCount = await _db.Outbox.CountAsync(e => e.Status == OutboxStatus.Pending, cancellationToken);
        if (pendingCount > 100)
        {
            _logger.LogWarning("HEALTH CHECK: Outbox backlog {PendingCount} pending events", pendingCount);
            return HealthCheckResult.Degraded($"Outbox backlog: {pendingCount}");
        }

        _logger.LogInformation("HEALTH CHECK: Outbox backlog OK ({PendingCount})", pendingCount);
        return HealthCheckResult.Healthy($"Outbox backlog: {pendingCount}");
    }
}
