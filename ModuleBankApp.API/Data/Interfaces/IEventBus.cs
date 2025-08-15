namespace ModuleBankApp.API.Data.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default);
}