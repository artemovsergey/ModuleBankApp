namespace ModuleBankApp.API.Domen.Events;
// ReSharper disable NotAccessedPositionalProperty.Global
/// <summary>
/// Разблокирование всех счетов клиента
/// </summary>
/// <param name="EventId"> Идентификатор события</param>
/// <param name="OccurredAt"> Дата</param>
/// <param name="ClientId"> Идентификатор клиента</param>
public record ClientUnblocked(
    Guid EventId,
    DateTime OccurredAt,
    Guid ClientId
): IEvent;