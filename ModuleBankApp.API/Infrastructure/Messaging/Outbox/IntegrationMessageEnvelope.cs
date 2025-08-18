namespace ModuleBankApp.API.Infrastructure.Messaging.Outbox;

public sealed record Meta(
    string Version,
    string Source,
    Guid CorrelationId,
    Guid CausationId
);

public sealed record IntegrationEventEnvelope<TPayload>(
    Guid EventId,
    string OccurredAt,   // ISO-8601 строка с суффиксом "Z"
    TPayload Payload,
    Meta Meta
)
{
    public static IntegrationEventEnvelope<TPayload> Create(
        TPayload payload,
        string source,
        Guid correlationId,
        Guid causationId)
    {
        return new IntegrationEventEnvelope<TPayload>(
            EventId: Guid.NewGuid(),
            OccurredAt: DateTime.UtcNow.ToString("O"), // всегда ISO-8601 с Z
            Payload: payload,
            Meta: new Meta(
                Version: "v1",
                Source: source,
                CorrelationId: correlationId,
                CausationId: causationId
            )
        );
    }
}


// что это и зачем нужно?