// ReSharper disable NotAccessedPositionalProperty.Global
namespace ModuleBankApp.API.Domen.Events;
/// <summary>
/// Событие создания счета
/// </summary>
/// <param name="EventId">EventId</param>
/// <param name="OccurredAt">OccurredAt</param>
/// <param name="AccountId">AccountId</param>
/// <param name="OwnerId">OwnerId</param>
/// <param name="Currency">Currency</param>
/// <param name="Type">Type</param>
public record AccountOpened(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    Guid OwnerId,
    string Currency,
    AccountType Type
) : IEvent;