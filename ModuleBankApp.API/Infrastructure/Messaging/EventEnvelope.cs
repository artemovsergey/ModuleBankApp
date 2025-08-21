namespace ModuleBankApp.API.Infrastructure.Messaging;

public sealed record Meta(
    string Version,
    string Source,
    Guid CorrelationId,
    Guid CausationId
);

public sealed record EventEnvelope<TPayload>(
    string OccurredAt,   // ISO-8601 строка с суффиксом "Z"
    TPayload Payload,
    Meta Meta
)
{
    public static EventEnvelope<TPayload> Create(
        TPayload payload,
        string source,
        Guid correlationId,
        Guid causationId)
    {
        return new EventEnvelope<TPayload>(
            OccurredAt: DateTime.UtcNow.ToString("O"),
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