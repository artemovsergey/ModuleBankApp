using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

public class RegisterTransactionHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ILogger<RegisterTransactionHandler> logger) : IRequestHandler<RegisterTransactionRequest, MbResult<Transaction>>
{
    public async Task<MbResult<Transaction>> Handle(RegisterTransactionRequest request, CancellationToken ct)
    {
        var t = new Transaction()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Type = request.TransactionDto.Type,
            AccountId = request.TransactionDto.AccountId,
            Amount = request.TransactionDto.Amount,
            Currency = request.TransactionDto.Currency,
            Description = request.TransactionDto.Description ?? ""
        };
        
        // изменяем аккаунт
        var account = await repoAccount.GetAccounById(t.AccountId);
        account.Balance += t.Amount;
        account.Currency = t.Currency;
        
        var result = await repoTransaction.RegisterTransaction(t);
        logger.LogWarning($"Creating transaction for account {result.AccountId}", request.ClaimsId);
        return MbResult<Transaction>.Success(result);
    }
}