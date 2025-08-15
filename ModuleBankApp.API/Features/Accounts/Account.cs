using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Features.Accounts;

/// <summary>
/// Модель банковского счёта.
/// </summary>
public sealed class Account
{
    /// <summary>
    /// Идентификатор (обязательное поле).
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Тип счета (перечисление).
    /// </summary>
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
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
    /// 
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Дата открытия счета
    /// </summary>
    ///
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата закрытия счета
    /// </summary>
    ///
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    // ReSharper disable once RedundantDefaultMemberInitializer
    public DateTime? ClosedAt { get; set; } = null!;

    /// <summary>
    /// Список транзакций по счету
    /// </summary>
    ///
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    // ReSharper disable once CollectionNeverUpdated.Global
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    /// <summary>
    /// Владелец счета
    /// </summary>
    ///
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid OwnerId { get; set; }
    
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    // ReSharper disable once UnusedMember.Global
    //public uint xmin { get; set; }
}

public enum AccountType
{
    Deposit = 0,
    Checking = 1,
    Credit = 2,
    None = 3
}

// +