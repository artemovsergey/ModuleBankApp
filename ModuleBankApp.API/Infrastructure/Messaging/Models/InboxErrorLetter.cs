namespace ModuleBankApp.API.Infrastructure.Messaging.Models;

public sealed class InboxErrorLetter
{
    public Guid Id { get; set; }
    public DateTimeOffset ReceivedAtUtc { get; set; }
    public string Handler { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}