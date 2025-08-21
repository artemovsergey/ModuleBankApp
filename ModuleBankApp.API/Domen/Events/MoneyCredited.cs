namespace ModuleBankApp.API.Domen.Events;

/// <summary>
/// Событие пополнения счета 
/// </summary>
public class MoneyCredited
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; set; }
    
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
    /// Валюта
    /// </summary>
    public string Currency { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор транзакции
    /// </summary>
    public Guid OperationId { get; set; }
    
}