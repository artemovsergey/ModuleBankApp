using ModuleBankApp.API.Domen;

namespace ModuleBankApp.API.Infrastructure.Data.Interfaces;

public interface IAccountRepository
{
    Task<Account> CreateAccount(Account acc);
    Task<Account> RemoveAccount(Guid id);
    Task<List<Account>> GetAllAccounts();
    Task<Account> GetAccountById(Guid id);
    Task<Account> UpdateAccount(Account acc, Guid accountId);
    Task<Account> FreezeAccount(Guid clientId, bool isFrozen);
}

