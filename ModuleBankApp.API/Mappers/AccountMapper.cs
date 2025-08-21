using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Features.Transactions;
using ModuleBankApp.API.Mappers;

namespace ModuleBankApp.API.Features.Accounts;

public static class AccountMapper
{
    public static AccountDto ToDto(this Account entity)
    {
        return new AccountDto(
            entity.Id,
            entity.Type,
            entity.Currency,
            entity.Balance,
            entity.InterestRate,
            entity.CreatedAt,
            entity.ClosedAt,
            entity.OwnerId,
            entity.IsFrozen,
            entity.Transactions.Select(t => t.ToDto()).ToList()
        );
    }
}

// +