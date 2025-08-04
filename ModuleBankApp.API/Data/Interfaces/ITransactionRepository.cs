using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Data.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> RegisterTransaction(Transaction transaction);
    Task<List<Transaction>> GetTransactionsByAccount(Guid accountId);
    Task<List<Transaction>> GetTransactions();
    
}