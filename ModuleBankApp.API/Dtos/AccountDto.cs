using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Features.Transactions;

// ReSharper disable NotAccessedPositionalProperty.Global

namespace ModuleBankApp.API.Dtos;

public record AccountDto(
    Guid Id,
    AccountType Type,
    string Currency,
    decimal Balance,
    decimal? InterestRate,
    DateTime CreatedAt,
    DateTime? ClosedAt,
    Guid OwnerId,
    bool IsFrozen,
    List<TransactionDto> Transactions
);

// +