namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

public static class RegisterTransactionMapper
{
    public static Transaction MapToTransaction(this TransactionDto registerTransactionDto)
    {
        var t = new Transaction()
        {
            Type = registerTransactionDto.Type,
            AccountId = registerTransactionDto.AccountId,
            Amount = registerTransactionDto.Amount,
            Currency = registerTransactionDto.Currency,
            Description = registerTransactionDto.Description ?? ""
        };

        return t;
    }
}