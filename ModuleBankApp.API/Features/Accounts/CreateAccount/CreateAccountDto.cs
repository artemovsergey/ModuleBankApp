namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class AccountDto
{
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