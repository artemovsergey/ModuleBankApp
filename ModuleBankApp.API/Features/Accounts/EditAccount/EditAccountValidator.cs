using FluentValidation;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

// ReSharper disable once UnusedType.Global
public class EditAccountValidator : AbstractValidator<EditAccountRequest>
{
    public EditAccountValidator(ICurrencyService currencyService)
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.EditAccountDto.Balance)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Balance cannot be negative");
        
        RuleFor(x => x.EditAccountDto.Type)
            .Must(x => x is AccountType.Credit or AccountType.Deposit or AccountType.Checking)
            .WithMessage("Тип счёта должен быть 'Credit' или 'Deposit' или 'Checked'");

        RuleFor(request => request.EditAccountDto.Currency)
            .NotEmpty()
            .WithMessage("Currency code is required.")
            .Length(3)
            .WithMessage("Currency code must be 3 characters long.")
            .Must(currencyService.IsValidCurrencyCode)
            .WithMessage("Invalid currency code. Please provide a valid ISO 4217 currency code.");

        When(x => x.EditAccountDto.Type == AccountType.Deposit, () =>
        {
            RuleFor(x => x.EditAccountDto.InterestRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Для вклада должна быть указана процентная ставка");
        });
        
        When(x => x.EditAccountDto.Type == AccountType.Credit, () =>
        {
            RuleFor(x => x.EditAccountDto.Balance)
                .LessThanOrEqualTo(0)
                .WithMessage("Credit account must have negative or zero balance");
        });
    }
}

// +