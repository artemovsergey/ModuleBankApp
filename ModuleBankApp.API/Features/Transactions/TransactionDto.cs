namespace ModuleBankApp.API.Features.Transactions;

/// <summary>
/// Модель передачи данных транзакции по счётам.
/// </summary>
public sealed class TransactionDto
{
    
    /// <summary>
    /// Идентификатор счета
    /// </summary>
    public Guid AccountId { get; init; }
    
    /// <summary>
    /// Cчёт контрагента, которая заполняется только для внутренних переводов между счетами.
    ///  Для внешних операций (платежи, пополнения) оно остаётся null.
    /// </summary>
    public Guid? CounterPartyAccountId { get; init; }
    
    /// <summary>
    /// Валюта
    /// </summary>
    public required string Currency { get; init; }
    
    /// <summary>
    /// Сумма транзакции
    /// </summary>
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Тип транзакции
    /// </summary>
    public TransactionType Type { get; init; }
    
    /// <summary>
    /// Описание транзакции
    /// </summary>
    public required string Description { get; init; }
    
}

// +