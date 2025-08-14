namespace ModuleBankApp.API.Features.Accounts.EditAccount;

public static class EditAccountMapper
{
    public static Account MapToAccount(this EditAccountDto editAccountDto)
    {
        // ReSharper disable once RedundantEmptyObjectCreationArgumentList
        var account = new Account()
        {
            Balance = editAccountDto.Balance,
            Type = editAccountDto.Type,
            InterestRate = editAccountDto.InterestRate,
            Currency = editAccountDto.Currency
        };

        return account;
    }
}

//+