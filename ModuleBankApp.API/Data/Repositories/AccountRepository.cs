using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Data.Interfaces;

namespace ModuleBankApp.API.Data.Repositories;

public class AccountRepository(ModuleBankAppContext db) : IAccountRepository
{
    public async Task<Account> CreateAccount(Account acc)
    {
        await db.Accounts.AddAsync(acc);
        await db.SaveChangesAsync();
        return acc;
    }

    public async Task<Account> UpdateAccount(Account acc, Guid accountId)
    {
        var result = await db.Accounts
            .Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(a => a.Type, acc.Type)
                .SetProperty(a => a.Currency, acc.Currency)
                .SetProperty(a => a.Balance, acc.Balance)
                .SetProperty(a => a.InterestRate, acc.InterestRate)
            );

        if (result == 0)
            throw new KeyNotFoundException("Account not found");

        return acc;
    }

    public async Task<Account> FreezeAccount(Guid clientId, bool isFrozen)
    {
        var c = await db.Accounts.Where(a => a.OwnerId == clientId).FirstOrDefaultAsync();
        c!.IsFrozen = isFrozen;
        await db.SaveChangesAsync();
        return c;
    }

    public async Task<List<Account>> GetAllAccounts()
    {
        return await db.Accounts.ToListAsync();
    }

    public async Task<Account> GetAccountById(Guid id)
    {
        var result = await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        return result!;
    }

    public async Task<Account> RemoveAccount(Guid id)
    {
        var acc = await db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if (acc == null) throw new KeyNotFoundException("Account not found");
        db.Accounts.Remove(acc);
        await db.SaveChangesAsync();
        return acc;
    }
}

// +