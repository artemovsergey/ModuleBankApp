using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Dtos;

namespace ModuleBankApp.API.Mappers;

public static class TransactionMapper
{
    public static TransactionDto ToDto(this Transaction entity)
    {
        // ReSharper disable once RedundantEmptyObjectCreationArgumentList
        return new TransactionDto()
        {
            AccountId = entity.AccountId,
            CounterPartyAccountId = entity.CounterPartyAccountId ?? Guid.Empty,
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
            CounterPartyAccountId = dto.CounterPartyAccountId ?? Guid.Empty,
            Currency = dto.Currency,
            Amount = dto.Amount,
            Type = dto.Type,
            Description = dto.Description
        };
    }
}

