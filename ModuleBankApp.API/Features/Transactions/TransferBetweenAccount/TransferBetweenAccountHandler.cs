using MediatR;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public class TransferBetweenAccountHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ModuleBankAppContext dbContext,
    ILogger<TransferBetweenAccountHandler> logger) : IRequestHandler<TransferBetweenAccountRequest, MbResult<TransactionDto>>
{
    public async Task<MbResult<TransactionDto>> Handle(TransferBetweenAccountRequest request, CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        try
        {
            var t = request.TransactionDto.ToEntity();
            var accountSender = await repoAccount.GetAccounById(t.AccountId);
            accountSender.Balance -= t.Amount;
            accountSender.Currency = t.Currency;
            var accountReceiver = await repoAccount.GetAccounById(t.CounterPartyAccountId);
            accountReceiver.Balance += t.Amount;
            accountReceiver.Currency = t.Currency;
            // Проверка итоговых балансов
            if (accountSender.Balance < 0 || accountReceiver.Balance < 0)
            {
                await transaction.RollbackAsync(ct);
                return MbResult<TransactionDto>.Failure("Некорректный итоговый баланс. Операция отменена.");
            }
            var result = await repoTransaction.RegisterTransaction(t);
            try
            {
                await dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(ct);
                return MbResult<TransactionDto>.Failure("Conflict");
            }
            logger.LogWarning($"Creating transfer between account {accountSender.Id} и {accountReceiver.Id}", request.ClaimsId);
            return MbResult<TransactionDto>.Success(result.ToDto());
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogError(ex, "Ошибка при переводе средств");
            return MbResult<TransactionDto>.Failure("Ошибка при переводе средств");
        }
    }
}