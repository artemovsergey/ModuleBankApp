using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Messaging.Models;

namespace ModuleBankApp.API.Handlers;

public record AccrueInterestRequest : IRequest<MbResult<bool>>;

public class AccrueInterestForAllHandler(
    ModuleBankAppContext db,
    ILogger<AccrueInterestForAllHandler> logger
) : IRequestHandler<AccrueInterestRequest, MbResult<bool>>
{
    public async Task<MbResult<bool>> Handle(AccrueInterestRequest request, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        try
        {
            var accountIds = await db.Accounts
                .Where(a => a.Type == AccountType.Deposit && a.InterestRate != null)
                .Select(a => a.Id)
                .ToListAsync(ct);

            foreach (var id in accountIds)
            {
                await db.Database.ExecuteSqlRawAsync(
                    "CALL public.accrue_interest({0})",
                    id
                );
                
                var @event = new InterestAccrued
                {
                    EventId = Guid.NewGuid(),
                    OccurredAt = DateTime.Now,
                    AccountId = id
                };
                
                await db.Outbox.AddAsync(new OutboxMessage
                {
                    Type = nameof(InterestAccrued),
                    Payload = JsonSerializer.Serialize(@event),
                    Status = OutboxStatus.Pending
                }, ct);

                await db.SaveChangesAsync(ct);
            }
            
            await transaction.CommitAsync(ct);

            logger.LogInformation("Interest accrued for {Count} accounts", accountIds.Count);
            return MbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogError(ex, "Error accruing interest for all accounts");
            return MbResult<bool>.Failure("Ошибка начисления процентов");
        }
    }
}

