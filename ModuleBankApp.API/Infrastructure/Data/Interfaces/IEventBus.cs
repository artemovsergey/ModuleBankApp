namespace ModuleBankApp.API.Infrastructure.Data.Interfaces;

public interface IEventBusService
{
    Task PublishAsync<T>(T @event, string routekey, CancellationToken cancellationToken = default);
}