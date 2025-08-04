using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Data.Interfaces;

namespace ModuleBankApp.API.Data.Repositories;

public class AccountMemoryRepository : IAccountRepository
{
    private List<Account> _accounts = new()
    {
        new Account()
        {
            Id = Guid.NewGuid(),
            Type = AccountType.Deposit,
            Currency = "USD",
            Balance = 1000.00m,
            InterestRate = 0.05m,
            CreatedAt = DateTime.UtcNow,
            OwnerId = Guid.NewGuid()
        },
        new Account()
            {
            Id = Guid.NewGuid(),
            Type = AccountType.Deposit,
            Currency = "USD",
            Balance = 2000.00m,
            InterestRate = 0.15m,
            CreatedAt = DateTime.UtcNow,
            OwnerId = Guid.NewGuid()
        }
    };

    public async Task<Account> CreateAccount(Account acc)
    {
        _accounts.Add(acc);
        return await Task.FromResult(acc);
    }

    public async Task<List<Account>> GetAllAccounts()
    {
        return await Task.FromResult(_accounts);
    }

    public async Task<Account> GetAccounById(Guid? id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        return await Task.FromResult(account!);
    }

    public async Task<Account> RemoveAccount(Guid id)
    {
        var acc = _accounts.FirstOrDefault(a => a.Id == id);
        await Task.FromResult(_accounts.Remove(acc!));

        return acc!;
    }
}