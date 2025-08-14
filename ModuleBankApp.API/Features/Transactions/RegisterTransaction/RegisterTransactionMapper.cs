namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

// ReSharper disable once UnusedType.Global
public static class RegisterTransactionMapper
{
    // ReSharper disable once UnusedMember.Global
    public static Transaction MapToTransaction(this TransactionDto registerTransactionDto)
    {
        // ReSharper disable once RedundantEmptyObjectCreationArgumentList
        var t = new Transaction()
        {
            Type = registerTransactionDto.Type,
            AccountId = registerTransactionDto.AccountId,
            Amount = registerTransactionDto.Amount,
            Currency = registerTransactionDto.Currency,
            Description = registerTransactionDto.Description
        };

        return t;
    }
}

// +