using System.Text.Json;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Messaging.Outbox;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class CreateAccountHandler(
    IAccountRepository repo,
    ILogger<CreateAccountHandler> logger,
    ModuleBankAppContext db,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<CreateAccountRequest, MbResult<Account>>
{
    public async Task<MbResult<Account>> Handle(CreateAccountRequest request, CancellationToken ct)
    {
        // ReSharper disable once RedundantEmptyObjectCreationArgumentList
        var account = new Account()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Type = request.CreateAccountDto.Type,
            Balance = request.CreateAccountDto.Balance,
            InterestRate = request.CreateAccountDto.InterestRate,
            Currency = request.CreateAccountDto.Currency,
            OwnerId = (Guid)request.ClaimsId! // from jwt::sub
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
        logger.LogInformation("Creating account {Id} for user {OwnerId}", savedAccount.Id, savedAccount.OwnerId);

        var httpContext = httpContextAccessor.HttpContext;

        Guid.TryParse(httpContext?.Items["X-Correlation-Id"]?.ToString(), out var correlationId);

        var causationId = request.ClaimsId ?? Guid.NewGuid();

        // конверт как пакет сообщения для rabbit
        var envelope = IntegrationEventEnvelope<AccountOpened>.Create(
            payload: @event,
            source: "account-service",
            correlationId: correlationId,
            causationId: causationId
        );

        await db.Outbox.AddAsync(new OutboxMessage
        {
            Id = envelope.EventId,
            Type = nameof(AccountOpened),
            Payload = JsonSerializer.Serialize(envelope),
            OccurredAtUtc = DateTimeOffset.UtcNow,
            Status = OutboxStatus.Pending,
            CorrelationId = correlationId
        }, ct);

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return MbResult<Account>.Success(savedAccount);
    }
}