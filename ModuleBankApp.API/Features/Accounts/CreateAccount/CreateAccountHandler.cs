using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class CreateAccountHandler(IAccountRepository repo, ILogger<CreateAccountHandler> logger) : IRequestHandler<CreateAccountRequest, MbResult<Account>>
{
    
    public async Task<MbResult<Account>> Handle(CreateAccountRequest request, CancellationToken ct)
    {
        var account = new Account()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Type = request.AccountDto.Type,
            Balance = request.AccountDto.Balance,
            InterestRate = request.AccountDto.InterestRate,
            Currency = request.AccountDto.Currency,
            OwnerId = request.ClaimsId // from jwt::sub
        };

        var result = await repo.CreateAccount(account);
        logger.LogInformation($"Creating account for user {result.OwnerId}", request.ClaimsId);
        return MbResult<Account>.Success(result);
    }
}