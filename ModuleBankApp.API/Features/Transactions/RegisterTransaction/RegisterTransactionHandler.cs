using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

public class RegisterTransactionHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ILogger<RegisterTransactionHandler> logger
) : IRequestHandler<RegisterTransactionRequest, MbResult<TransactionDto>>
{
    public async Task<MbResult<TransactionDto>> Handle(RegisterTransactionRequest request, CancellationToken ct)
    {
        var transactionEntity = request.TransactionDto.ToEntity();
        
        var account = await repoAccount.GetAccounById(transactionEntity.AccountId);
        account.Balance += transactionEntity.Amount;
        account.Currency = transactionEntity.Currency;
        
        var savedTransaction = await repoTransaction.RegisterTransaction(transactionEntity);
        logger.LogWarning($"Creating transaction for account {savedTransaction.AccountId}", request.ClaimsId);
        
        await repoAccount.UpdateAccount(account, account.Id);
        logger.LogWarning($"Update state account {account.Id}", request.ClaimsId);

        return MbResult<TransactionDto>.Success(savedTransaction.ToDto());
    }
}