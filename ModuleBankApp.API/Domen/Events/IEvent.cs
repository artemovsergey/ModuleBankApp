namespace ModuleBankApp.API.Domen.Events;

public interface IEvent
{
    Guid EventId { get; init; }
}