using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

public class EditAccountHandler(IAccountRepository repo, ILogger<EditAccountHandler> logger) : IRequestHandler<EditAccountRequest, MbResult<Account>>
{
    
    public async Task<MbResult<Account>> Handle(EditAccountRequest request, CancellationToken ct)
    {
        var currentAccount = await repo.GetAccounById(request.AccountId);

        currentAccount.Currency = request.EditAccountDto.Currency;
        currentAccount.Balance = request.EditAccountDto.Balance;
        currentAccount.InterestRate = request.EditAccountDto.InterestRate;
        currentAccount.Type = request.EditAccountDto.Type;
        
        logger.LogInformation($"Update account {currentAccount.Id}");
        return MbResult<Account>.Success(currentAccount);
    }
}