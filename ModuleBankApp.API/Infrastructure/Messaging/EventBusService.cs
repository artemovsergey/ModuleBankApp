using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using RabbitMQ.Client;

namespace ModuleBankApp.API.Infrastructure.Messaging;

public class EventBusService(IEventBusConnection eventBusConnection,
                      IOptions<EventBusOptions> options,
                      IHttpContextAccessor httpContextAccessor)
    : IEventBusService
{
    
    public async Task PublishAsync<T>(T @event, string routeKey, CancellationToken cancellationToken = default)
    {
        await using var channel = await eventBusConnection.CreateChannelAsync();
        
        var httpContext = httpContextAccessor.HttpContext;
        Guid.TryParse(httpContext?.Items["X-Correlation-Id"]?.ToString(), out var correlationId);
        
        var ownerIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid.TryParse(ownerIdClaim, out var causationId);
        
        var envelope = EventEnvelope<T>.Create(
            payload: @event,
            source: "account-service",
            correlationId: correlationId,
            causationId: causationId
        );
        
        var props = new BasicProperties();
        props.Persistent = true;
        props.Headers = new Dictionary<string, object>
        {
            { "X-Correlation-Id", envelope.Meta.CorrelationId.ToString() },
            { "X-Causation-Id", envelope.Meta.CausationId.ToString() },
            { "Source", envelope.Meta.Source },
            { "Version", envelope.Meta.Version }
        }!;
        
        var json = JsonSerializer.Serialize(envelope);
        var body = Encoding.UTF8.GetBytes(json).AsMemory();
        
        await channel.BasicPublishAsync(
            exchange: options.Value.ExchangeName,
            routingKey: routeKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }

}

