using System.Text.Json;
using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Models;
using ModuleBankApp.API.Mappers;

namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

public class RegisterTransactionHandler(
    ITransactionRepository repoTransaction,
    IAccountRepository repoAccount,
    ModuleBankAppContext db,
    ILogger<RegisterTransactionHandler> logger
) : IRequestHandler<RegisterTransactionRequest, MbResult<TransactionDto>>
{
    public async Task<MbResult<TransactionDto>> Handle(RegisterTransactionRequest request, CancellationToken ct)
    {
        var transactionEntity = request.TransactionDto.ToEntity();

        IEvent @event;
        if (request.TransactionDto.Type == TransactionType.Credit)
        {
            @event = new MoneyCredited()  
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.Now,
                AccountId = transactionEntity.AccountId,
                Amount = transactionEntity.Amount,
                Currency = transactionEntity.Currency,
                OperationId = transactionEntity.Id
            };
        }
        else
        {
            @event = new MoneyDebited()  
            {
                EventId = Guid.NewGuid(),
                OccurredAt = DateTime.Now,
                AccountId = transactionEntity.AccountId,
                Amount = transactionEntity.Amount,
                Currency = transactionEntity.Currency,
                OperationId = transactionEntity.Id
            }; 
        }
        
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        
        await db.Outbox.AddAsync(new OutboxMessage
        {
            Id = @event.EventId,
            Type = request.TransactionDto.Type == TransactionType.Credit ? nameof(MoneyCredited) : nameof(MoneyDebited),
            Payload = JsonSerializer.Serialize(@event),
            Status = OutboxStatus.Pending
        }, ct);
        
        var account = await repoAccount.GetAccountById(transactionEntity.AccountId);
        
        switch (request.TransactionDto.Type)
        {
            case TransactionType.Credit:
                account.Balance += transactionEntity.Amount;
                break;
            case TransactionType.Debit:
                account.Balance -= transactionEntity.Amount;
                break;
        }

        var savedTransaction = await repoTransaction.RegisterTransaction(transactionEntity);
        logger.LogWarning("Creating transaction for account {savedTransaction.AccountId}", savedTransaction.AccountId);
        
        await repoAccount.UpdateAccount(account, account.Id);
        logger.LogWarning("Update state account {account.Id}", account.Id);
        
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        
        return MbResult<TransactionDto>.Success(savedTransaction.ToDto());
    }
}