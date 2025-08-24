using System.Text.Json;
using Hangfire.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Models;
using ModuleBankApp.API.Mappers;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public class TransferBetweenAccountHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ModuleBankAppContext db,
    ModuleBankAppContext dbContext,
    ILogger<TransferBetweenAccountHandler> logger)
    : IRequestHandler<TransferBetweenAccountRequest, MbResult<TransactionDto>>
{
    public async Task<MbResult<TransactionDto>> Handle(TransferBetweenAccountRequest request, CancellationToken ct)
    {
        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

        try
        {
            var t = request.TransactionDto.ToEntity();

            var @event = new TransferCompleted()
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.Now,
                SourceAccountId = t.AccountId,
                DestinationAccountId = t.CounterPartyAccountId,
                Amount = t.Amount,
                Currency = t.Currency,
                TransferId = t.Id
            };

            await db.Outbox.AddAsync(new OutboxMessage
            {
                Id = @event.EventId,
                Type = nameof(TransferCompleted),
                Payload = JsonSerializer.Serialize(@event),
                Status = OutboxStatus.Pending
            }, ct);


            var accountSender = await repoAccount.GetAccountById(t.AccountId);
            accountSender.Balance -= t.Amount;
            //db.Entry(accountSender).State = EntityState.Modified;
            
            var accountReceiver = await repoAccount.GetAccountById((Guid)t.CounterPartyAccountId!);
            accountReceiver.Balance += t.Amount;
            //db.Entry(accountReceiver).State = EntityState.Modified;
            
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

            logger.LogWarning("Creating transfer between account {accountSender.Id} и {accountReceiver.Id}",
                accountSender.Id, accountReceiver.Id);
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