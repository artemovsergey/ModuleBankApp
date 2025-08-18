using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

public class EditAccountHandler(IAccountRepository repo, ILogger<EditAccountHandler> logger) : IRequestHandler<EditAccountRequest, MbResult<Account>>
{
    
    public async Task<MbResult<Account>> Handle(EditAccountRequest request, CancellationToken ct)
    {
        var account = request.EditAccountDto.MapToAccount();
        var currentAccount = await repo.UpdateAccount(account, request.AccountId);
        
        logger.LogInformation("Update account {currentAccountId}", currentAccount.Id);
        return MbResult<Account>.Success(currentAccount);
    }
}

//+