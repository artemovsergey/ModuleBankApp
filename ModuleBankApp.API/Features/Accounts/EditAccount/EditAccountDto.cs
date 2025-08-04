namespace ModuleBankApp.API.Features.Accounts.EditAccount;

/// <summary>
/// Модель передачи данных при редактировании счета.
/// </summary>
public class EditAccountDto
{
    /// <summary>
    /// Id .
    /// </summary>
    public Guid Id { get; set; }
    
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
    
}