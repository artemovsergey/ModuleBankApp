// ReSharper disable UnusedAutoPropertyAccessor.Global

using ModuleBankApp.API.Domen;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

/// <summary>
/// Модель передачи данных при редактировании счета.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class EditAccountDto
{
    /// <summary>
    /// Id
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Тип счета (перечисление).
    /// </summary>
    public required AccountType Type { get; set; }
    
    /// <summary>
    /// Валюта
    /// </summary>
    public required string Currency { get; set; }
    
    /// <summary>
    /// Баланс
    /// </summary>
    public required decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка
    /// </summary>
    public decimal? InterestRate { get; set; }
    
    /// <summary>
    /// Xmin в PostgreSQL
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public required uint Xmin { get; set; }
    
}

// +