using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Data.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> RegisterTransaction(Transaction transaction);
    Task<List<Transaction>> GetTransactionsByAccount(Guid accountId);
    
    // ReSharper disable once UnusedMember.Global
    Task<List<Transaction>> GetTransactions();
    
}