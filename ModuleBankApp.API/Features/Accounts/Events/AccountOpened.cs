namespace ModuleBankApp.API.Features.Accounts.Events;

public record AccountOpened(
    Guid AccountId,
    Guid OwnerId,
    string Currency,
    AccountType Type,
    decimal Balance,
    decimal? InterestRate,
    DateTime CreatedAt
);