namespace ModuleBankApp.API.Domen.Events;

/// <summary>
/// Блокирование всех счетов клиента
/// </summary>
/// <param name="EventId"> Идентификатор события</param>
/// <param name="OccurredAt"> Дата</param>
/// <param name="ClientId"> Идентификатор клиента</param>
public record ClientBlocked(
    Guid EventId,
    DateTime OccurredAt,
    Guid ClientId
);