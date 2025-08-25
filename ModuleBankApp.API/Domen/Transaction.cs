// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace ModuleBankApp.API.Domen;

/// <summary>
/// Модель транзакции по счётам.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Идентификатор транзакции
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор счета
    /// </summary>
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }

    /// <summary>
    /// Cчёт контрагента, которая заполняется только для внутренних переводов между счетами.
    ///  Для внешних операций (платежи, пополнения) оно остаётся null.
    /// </summary>
    public Guid? CounterPartyAccountId { get; set; }

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
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Время транзакции
    /// </summary>
    public DateTime CreatedAt { get; set; }
}


/// <summary>
/// Модель типов транзакции
/// </summary>
public enum TransactionType
{
    Credit = 0,
    Debit = 1
}