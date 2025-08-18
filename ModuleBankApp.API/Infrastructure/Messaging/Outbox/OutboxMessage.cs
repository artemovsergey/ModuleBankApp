namespace ModuleBankApp.API.Infrastructure.Messaging.Outbox;

public sealed class OutboxMessage : IHasCorrelation, IHasCausation
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Type { get; init; } = default!;
    public string Payload { get; init; } = default!;  // JSON строка
    public DateTimeOffset OccurredAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
    public int PublishAttempts { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public string? Error { get; set; }
    
    public Guid CorrelationId { get; set; }
    public Guid CausationId { get; set; }
}