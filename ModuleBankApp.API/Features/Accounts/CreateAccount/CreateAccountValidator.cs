using FluentValidation;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

// ReSharper disable once UnusedType.Global
public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator(ICurrencyService currencyService)
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.CreateAccountDto.Balance)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Balance cannot be negative");
        
        RuleFor(x => x.CreateAccountDto.Type)
            .Must(x => x is AccountType.Credit or AccountType.Deposit or AccountType.Checking)
            .WithMessage("Тип счёта должен быть 'Credit' или 'Deposit' или 'Checked'");

        RuleFor(request => request.CreateAccountDto.Currency)
            .NotEmpty()
            .WithMessage("Currency code is required.")
            .Length(3)
            .WithMessage("Currency code must be 3 characters long.")
            .Must(currencyService.IsValidCurrencyCode)
            .WithMessage("Invalid currency code. Please provide a valid ISO 4217 currency code.");

        When(x => x.CreateAccountDto.Type is AccountType.Deposit or AccountType.Credit, () =>
        {
            RuleFor(x => x.CreateAccountDto.InterestRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Для вклада должна быть указана процентная ставка");
        });
        
        RuleFor(x => x.CreateAccountDto.Type)
            .Must(type => Enum.IsDefined(type) && type != AccountType.None)
            .WithMessage("Недопустимый тип счёта");
        
        When(x => x.CreateAccountDto.Type == AccountType.Credit, () =>
        {
            RuleFor(x => x.CreateAccountDto.Balance)
                .LessThanOrEqualTo(0)
                .WithMessage("Credit account must have negative or zero balance");
        });
    }
}

// +