using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

public class RemoveAccountHandler(IAccountRepository repo, ILogger<RemoveAccountHandler> logger) : IRequestHandler<RemoveAccountRequest, MbResult<Account>>
{
    
    public async Task<MbResult<Account>> Handle(RemoveAccountRequest request, CancellationToken ct)
    {
        var result = await repo.RemoveAccount(request.AccountId);
        logger.LogInformation("Remove account {Id}", result.Id);
        return MbResult<Account>.Success(result);
    }
}

// +