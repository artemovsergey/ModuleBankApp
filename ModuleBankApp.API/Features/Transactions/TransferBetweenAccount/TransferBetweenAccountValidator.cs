using FluentValidation;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

// ReSharper disable once UnusedType.Global
public class TransferBetweenAccountValidator : AbstractValidator<TransferBetweenAccountRequest>
{
    public TransferBetweenAccountValidator(ICurrencyService currencyService)
    {
        
        RuleFor(request => request.TransactionDto.Amount)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Amount cannot be negative");
        
        RuleFor(x => x.TransactionDto.Type)
            .Must(x => x is TransactionType.Credit or TransactionType.Debit)
            .WithMessage("Тип транзакции должен быть 'Debit' или 'Credit'");

        RuleFor(request => request.TransactionDto.Currency)
            .NotEmpty()
            .WithMessage("Currency code is required.")
            .Length(3)
            .WithMessage("Currency code must be 3 characters long.")
            .Must(currencyService.IsValidCurrencyCode)
            .WithMessage("Invalid currency code. Please provide a valid ISO 4217 currency code.");
    }
}
// +