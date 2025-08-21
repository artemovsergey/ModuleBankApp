namespace ModuleBankApp.API.Domen.Events;

/// <summary>
/// Событие снятия средств со счета 
/// </summary>
public class MoneyDebited
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
    /// Сумма снятия средств со счета
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
    
    /// <summary>
    /// Причина снятия 
    /// </summary>
    public string? Reason { get; set; }
}