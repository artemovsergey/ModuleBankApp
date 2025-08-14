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
    ILogger<TransferBetweenAccountHandler> logger)
    : IRequestHandler<TransferBetweenAccountRequest, MbResult<TransactionDto>>
{
    public async Task<MbResult<TransactionDto>> Handle(TransferBetweenAccountRequest request, CancellationToken ct)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, ct);

        try
        {
            var t = request.TransactionDto.ToEntity();

            var accountSender = await repoAccount.GetAccountById(t.AccountId);
            var accountReceiver = await repoAccount.GetAccountById(t.CounterPartyAccountId!.Value);

            accountReceiver.Balance += t.Amount;
            accountSender.Balance -= t.Amount;

            var result = await repoTransaction.RegisterTransaction(t);

            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            logger.LogInformation(
                "Transfer between accounts {SenderId} -> {ReceiverId}, Amount: {Amount} {Currency}",
                accountSender.Id, accountReceiver.Id, t.Amount, t.Currency
            );

            return MbResult<TransactionDto>.Success(result.ToDto());
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            return MbResult<TransactionDto>.Failure("Conflict");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogError(ex, "Ошибка при переводе средств");
            return MbResult<TransactionDto>.Failure("Ошибка при переводе средств");
        }
    }
}


// +