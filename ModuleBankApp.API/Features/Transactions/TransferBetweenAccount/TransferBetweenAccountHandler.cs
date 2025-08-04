using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public class TransferBetweenAccountHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ILogger<TransferBetweenAccountHandler> logger) : IRequestHandler<TransferBetweenAccountRequest, MbResult<Transaction>>
{
    public async Task<MbResult<Transaction>> Handle(TransferBetweenAccountRequest request, CancellationToken ct)
    {
        var t = new Transaction()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Type = request.TransactionDto.Type,
            
            AccountId = request.TransactionDto.AccountId,
            CounterPartyAccountId = request.TransactionDto.CounterPartyAccountId,
            
            Amount = request.TransactionDto.Amount,
            Currency = request.TransactionDto.Currency,
            Description = request.TransactionDto.Description ?? ""
        };
        
        // изменяем аккаунт отравителя
        var accountSender = await repoAccount.GetAccounById(t.AccountId);
        accountSender.Balance -= t.Amount;
        accountSender.Currency = t.Currency;
        
        // изменяем аккаунт получателя
        var accountReceiver = await repoAccount.GetAccounById(t.CounterPartyAccountId);
        accountReceiver.Balance += t.Amount;
        accountReceiver.Currency = t.Currency;
        
        var result = await repoTransaction.RegisterTransaction(t);
        logger.LogWarning($"Creating transfer between account {accountSender.Id} и {accountReceiver.Id}", request.ClaimsId);
        return MbResult<Transaction>.Success(result);
    }
}