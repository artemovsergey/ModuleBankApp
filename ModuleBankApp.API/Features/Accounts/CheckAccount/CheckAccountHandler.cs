using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

public class CheckAccountHandler(IAccountRepository repo, ILogger<CheckAccountHandler> logger)
    : IRequestHandler<CheckAccountRequest, MbResult<AccountDto>>
{
    public async Task<MbResult<AccountDto>> Handle(CheckAccountRequest request, CancellationToken ct)
    {
        var account = await repo.GetAccountById(request.AccountId);

        if (account == null!)
        {
            logger.LogInformation("Account with Id: {accountId} not found.", request.AccountId);
            return MbResult<AccountDto>.Failure("Account not found.");
        }

        if (account.OwnerId != request.OwnerId)
        {
            logger.LogWarning("Forbidden access to account with Id: {AccountId} for OwnerId: {OwnerId}.", request.AccountId, request.OwnerId);
            return MbResult<AccountDto>.Failure("Forbidden");
        }
        
        logger.LogInformation("Account with Id: {AccountId} found for OwnerId: {OwnerId}.",request.AccountId,request.OwnerId);
        return MbResult<AccountDto>.Success(account.ToDto());
    }
}

// +