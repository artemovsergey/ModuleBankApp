using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public class TransferBetweenAccountHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ILogger<TransferBetweenAccountHandler> logger) : IRequestHandler<TransferBetweenAccountRequest, MbResult<TransactionDto>>
{
    public async Task<MbResult<TransactionDto>> Handle(TransferBetweenAccountRequest request, CancellationToken ct)
    {
        var t = request.TransactionDto.ToEntity();
        
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
        return MbResult<TransactionDto>.Success(result.ToDto());
    }
}