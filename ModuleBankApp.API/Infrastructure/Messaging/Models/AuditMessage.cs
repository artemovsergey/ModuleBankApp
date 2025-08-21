namespace ModuleBankApp.API.Infrastructure.Messaging.Models;

public record AuditMessage(Guid Id, string Type, string Payload, DateTimeOffset ReceivedAtUtc);