namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

/// <summary>
/// Модель передачи данных транзакции по счётам.
/// </summary>
public class TransactionTransferDto
{
    
    /// <summary>
    /// Идентификатор счета
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Cчёт контрагента, которая заполняется только для внутренних переводов между счетами.
    ///  Для внешних операций (платежи, пополнения) оно остаётся null.
    /// </summary>
    public Guid? CounterPartyAccountId { get; set; } = null;

    /// <summary>
    /// Валюта
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Сумма транзакции
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Тип транзакции
    /// </summary>
    public TransactionType Type { get; set; }

    /// <summary>
    /// Описание транзакции
    /// </summary>
    public string? Description { get; set; } = string.Empty;
    
}
