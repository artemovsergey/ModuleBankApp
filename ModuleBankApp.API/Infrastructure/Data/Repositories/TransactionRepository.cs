using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen;

namespace ModuleBankApp.API.Infrastructure.Data.Repositories;

public class TransactionRepository(ModuleBankAppContext context) : ITransactionRepository
{
    public async Task<Transaction> RegisterTransaction(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<List<Transaction>> GetTransactions()
    {
        return await context.Transactions.ToListAsync();
    }

    public async Task<List<Transaction>> GetTransactionsByAccount(Guid accountId)
    {
        return await context.Transactions
            .Where(t => t.AccountId == accountId)
            .ToListAsync();
    }
}

// +