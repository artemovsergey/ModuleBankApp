namespace ModuleBankApp.API.Domen.Events;
// ReSharper disable NotAccessedPositionalProperty.Global
/// <summary>
/// Событие пополнения счета 
/// </summary>
public class TransferCompleted : IEvent
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
    public Guid SourceAccountId { get; set; }
    
    /// <summary>
    /// Идентификатор счета пользователя
    /// </summary>
    public Guid DestinationAccountId { get; set; }
    
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
    public Guid TransferId { get; set; }
}