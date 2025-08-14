using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Features.Accounts;

public record AccountDto(
    Guid Id,
    AccountType Type,
    string Currency,
    decimal Balance,
    decimal? InterestRate,
    DateTime CreatedAt,
    DateTime? ClosedAt,
    Guid OwnerId,
    List<TransactionDto> Transactions
);