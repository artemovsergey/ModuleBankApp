using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;

namespace ModuleBankApp.API.Features.Accounts.GetAllAccounts;

public class GetAllAccountsHandler(IAccountRepository repo, ILogger<GetAllAccountsHandler> logger) : IRequestHandler<GetAllAccountsRequest, MbResult<List<Account>>>
{
    
    public async Task<MbResult<List<Account>>> Handle(GetAllAccountsRequest request, CancellationToken ct)
    {
        var result = await repo.GetAllAccounts();
        logger.LogInformation("Получение списка всех счетов");
        return MbResult<List<Account>>.Success(result);
    }
}

// +