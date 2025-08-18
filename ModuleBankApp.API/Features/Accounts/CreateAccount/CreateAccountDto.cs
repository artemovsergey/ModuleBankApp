using ModuleBankApp.API.Domen;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public record CreateAccountDto(
    AccountType Type,
    string Currency,
    decimal Balance,
    decimal? InterestRate
);