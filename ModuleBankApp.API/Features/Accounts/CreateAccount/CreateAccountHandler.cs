using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Features.Accounts.Events;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class CreateAccountHandler(IAccountRepository repo,
    ILogger<CreateAccountHandler> logger,
    IEventBus eventBus)
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
        logger.LogInformation("Creating account {account.Id} for user {result.OwnerId}", account.Id, result.OwnerId);
        
        // Публикуем событие
        var accountOpenedEvent = new AccountOpened(
            result.Id,
            result.OwnerId,
            result.Currency,
            result.Type,
            result.Balance,
            result.InterestRate,
            result.CreatedAt
        );
        
        await eventBus.PublishAsync(accountOpenedEvent, ct);
        logger.LogInformation("Событие создания счета отправилось в RabbitMQ");
        
        return MbResult<Account>.Success(result);
    }
}

// +