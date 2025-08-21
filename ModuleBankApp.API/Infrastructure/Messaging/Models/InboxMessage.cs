namespace ModuleBankApp.API.Infrastructure.Messaging.Models;

public sealed class InboxMessage
{
    public Guid Id { get; set; }  // EventId
    public string? Type { get; set; } = string.Empty;
    public string Payload { get; set; } = null!;
    public DateTimeOffset ReceivedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public bool Processed { get; set; } = false;
}