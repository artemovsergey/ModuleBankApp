using MediatR;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Handlers;

public record AccrueInterestRequest(Guid AccountId) : IRequest<MbResult<bool>>;

public class AccrueInterestHandler(
    ModuleBankAppContext db,
    ILogger<AccrueInterestHandler> logger
) : IRequestHandler<AccrueInterestRequest, MbResult<bool>>
{
    public async Task<MbResult<bool>> Handle(AccrueInterestRequest request, CancellationToken ct)
    {
        using var transaction = await db.Database.BeginTransactionAsync(ct);

        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "CALL public.accrue_interest({0})",
                request.AccountId
            );

            await transaction.CommitAsync(ct);

            logger.LogInformation("Interest accrued for account {AccountId}", request.AccountId);
            return MbResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogError(ex, "Error accruing interest for account {AccountId}", request.AccountId);
            return MbResult<bool>.Failure("Ошибка начисления процентов");
        }
    }
}