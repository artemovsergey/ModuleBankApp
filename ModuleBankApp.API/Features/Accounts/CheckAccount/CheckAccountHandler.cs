using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

public class CheckAccountHandler(IAccountRepository repo, ILogger<CheckAccountHandler> logger)
    : IRequestHandler<CheckAccountRequest, MbResult<Account>>
{
    public async Task<MbResult<Account>> Handle(CheckAccountRequest request, CancellationToken ct)
    {
        var result = await repo.GetAccounById(request.AccountId);
        logger.LogInformation($"Get account by Id:  {request.AccountId}");
        return MbResult<Account>.Success(result);
    }
}