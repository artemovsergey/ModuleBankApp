namespace ModuleBankApp.API.Infrastructure.Messaging.Outbox;

public enum OutboxStatus : short
{
    Pending = 0,
    InProgress = 1,
    Published = 2,
    Failed = 3
}