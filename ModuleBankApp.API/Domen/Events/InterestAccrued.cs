namespace ModuleBankApp.API.Domen.Events;
// ReSharper disable NotAccessedPositionalProperty.Global
/// <summary>
/// Событие пополнения счета 
/// </summary>
public class InterestAccrued : IEvent
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; init; }
    
    /// <summary>
    /// Время создания события
    /// </summary>
    public DateTime OccurredAt { get; set; }
    
    /// <summary>
    /// Идентификатор счета пользователя
    /// </summary>
    public Guid AccountId { get; set; }
    
    /// <summary>
    /// Сумма пополнения счета
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Дата начала начисления
    /// </summary>
    public DateTime PeriodFrom { get; set; }
    
    /// <summary>
    /// Дата окончания начисления
    /// </summary>
    public DateTime PeriodTo { get; set; }
    
    /// <summary>
    /// Валюта
    /// </summary>
    public string Currency { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор транзакции
    /// </summary>
    public Guid OperationId { get; set; }
}