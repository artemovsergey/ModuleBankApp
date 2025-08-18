namespace ModuleBankApp.API.Infrastructure.Data.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default);
}