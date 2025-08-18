namespace ModuleBankApp.API.Infrastructure.Messaging.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; set; }  // EventId
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTimeOffset ReceivedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public bool Processed { get; set; } = false;
}