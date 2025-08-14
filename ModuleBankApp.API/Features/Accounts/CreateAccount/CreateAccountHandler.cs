using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class CreateAccountHandler(IAccountRepository repo, ILogger<CreateAccountHandler> logger)
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

        var result = await repo.CreateAccount(account);
        logger.LogInformation("Creating account {account.Id} for user {result.OwnerId}",account.Id,result.OwnerId);
        return MbResult<Account>.Success(result);
    }
}

// +