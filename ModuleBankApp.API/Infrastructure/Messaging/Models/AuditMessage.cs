namespace ModuleBankApp.API.Infrastructure.Messaging.Models;

public record AuditMessage(Guid Id, string Payload, DateTimeOffset ReceivedAtUtc);