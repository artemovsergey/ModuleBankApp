using System.Text.Json;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Models;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class CreateAccountHandler(
    IAccountRepository repo,
    ILogger<CreateAccountHandler> logger,
    ModuleBankAppContext db)
    : IRequestHandler<CreateAccountRequest, MbResult<Account>>
{
    public async Task<MbResult<Account>> Handle(CreateAccountRequest request, CancellationToken ct)
    {
        // ReSharper disable once RedundantEmptyObjectCreationArgumentList
        var account = new Account()
        {
            Type = request.CreateAccountDto.Type,
            Balance = request.CreateAccountDto.Balance,
            InterestRate = request.CreateAccountDto.InterestRate,
            Currency = request.CreateAccountDto.Currency,
            OwnerId = (Guid)request.ClaimsId!
        };

        var @event = new AccountOpened(
            Guid.NewGuid(),
            account.CreatedAt,
            account.Id,
            account.OwnerId,
            account.Currency,
            account.Type
        );

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var savedAccount = await repo.CreateAccount(account);
        logger.LogInformation("\n Creating account {Id} for user {OwnerId} \n", savedAccount.Id, savedAccount.OwnerId);
        
        await db.Outbox.AddAsync(new OutboxMessage
        {
            Type = nameof(AccountOpened),
            Payload = JsonSerializer.Serialize(@event),
            Status = OutboxStatus.Pending
        }, ct);

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return MbResult<Account>.Success(savedAccount);
    }
}