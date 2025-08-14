namespace ModuleBankApp.API.Features.Transactions;

public static class TransactionMapper
{
    public static TransactionDto ToDto(this Transaction entity)
    {
        // ReSharper disable once RedundantEmptyObjectCreationArgumentList
        return new TransactionDto()
        {
            AccountId = entity.AccountId,
            CounterPartyAccountId = entity.CounterPartyAccountId,
            Currency = entity.Currency,
            Amount = entity.Amount,
            Type = entity.Type,
            Description = entity.Description
        };
    }

    public static Transaction ToEntity(this TransactionDto dto)
    {
        return new Transaction
        {
            AccountId = dto.AccountId,
            CounterPartyAccountId = dto.CounterPartyAccountId,
            Currency = dto.Currency,
            Amount = dto.Amount,
            Type = dto.Type,
            Description = dto.Description
        };
    }
}

// +