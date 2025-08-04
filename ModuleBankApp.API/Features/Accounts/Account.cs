using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Features.Accounts;

/// <summary>
/// Модель банковского счёта.
/// </summary>
public class Account
{
    /// <summary>
    /// Идентификатор (обязательное поле).
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Тип счета (перечисление).
    /// </summary>
    public AccountType Type { get; set; }
    
    /// <summary>
    /// Валюта
    /// </summary>
    public required string Currency { get; set; }
    
    /// <summary>
    /// Баланс
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка
    /// </summary>
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Дата открытия счета
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата закрытия счета
    /// </summary>
    public DateTime? ClosedAt { get; set; } = null;

    /// <summary>
    /// Список транзакций по счету
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    /// <summary>
    /// Владелец счета
    /// </summary>
    public Guid OwnerId { get; set; }
}

public enum AccountType
{
    Deposit,
    Checking,
    Credit,
    None
}