namespace ModuleBankApp.API.Infrastructure.Messaging.Models;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;  // JSON строка
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
}

public enum OutboxStatus : short
{
    Pending = 0,
    InProgress = 1,
    Published = 2,
    Failed = 3
}