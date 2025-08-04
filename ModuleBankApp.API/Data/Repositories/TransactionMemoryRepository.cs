using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Data.Repositories;

public class TransactionMemoryRepository : ITransactionRepository
{
    private List<Transaction> _transactions = new();
    
    public async Task<Transaction> RegisterTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
        return await Task.FromResult(transaction);
    }
    
    public async Task<List<Transaction>> GetTransactions()
    {
        return await Task.FromResult(_transactions);
    }

    public async Task<List<Transaction>> GetTransactionsByAccount(Guid accountId)
    {
        var result = _transactions.Where(t => t.AccountId == accountId).ToList();
        return await Task.FromResult(result);
    }
}