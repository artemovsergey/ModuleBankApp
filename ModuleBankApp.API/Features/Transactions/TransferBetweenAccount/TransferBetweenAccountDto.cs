namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

/// <summary>
/// Модель передачи данных транзакции по счётам.
/// </summary>
public sealed class TransactionTransferDto
{
    
    /// <summary>
    /// Идентификатор счета
    /// </summary>
    public required Guid AccountId { get; set; }

    /// <summary>
    /// Cчёт контрагента, которая заполняется только для внутренних переводов между счетами.
    ///  Для внешних операций (платежи, пополнения) оно остаётся null.
    /// </summary>
    public Guid? CounterPartyAccountId { get; set; } = null;

    /// <summary>
    /// Валюта
    /// </summary>
    public required string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Сумма транзакции
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    /// Тип транзакции
    /// </summary>
    public required TransactionType Type { get; set; }

    /// <summary>
    /// Описание транзакции
    /// </summary>
    public string? Description { get; set; } = string.Empty;
    
}
