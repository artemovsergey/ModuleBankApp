using ModuleBankApp.API.Features.Accounts;

namespace ModuleBankApp.API.Data.Interfaces;

public interface IAccountRepository
{
    Task<Account> CreateAccount(Account acc);
    Task<Account> RemoveAccount(Guid id);
    Task<List<Account>> GetAllAccounts();
    Task<Account> GetAccounById(Guid? id);

    Task<Account> UpdateAccount(Account acc, Guid accountId);
}