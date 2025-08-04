using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator(ICurrencyService currencyService)
    {
        ClassLevelCascadeMode = CascadeMode.Continue; // Проверять все поля, даже если есть ошибки
        RuleLevelCascadeMode = CascadeMode.Stop; // По каждому полю — первая ошибка

        RuleFor(request => request.AccountDto.Balance)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Balance cannot be negative");
        
        RuleFor(x => x.AccountDto.Type)
            .Must(x => x is AccountType.Credit or AccountType.Deposit or AccountType.Checking)
            .WithMessage("Тип счёта должен быть 'Credit' или 'Deposit' или 'Checked'");

        RuleFor(request => request.AccountDto.Currency)
            .NotEmpty()
            .WithMessage("Currency code is required.")
            .Length(3)
            .WithMessage("Currency code must be 3 characters long.")
            .Must(currencyService.IsValidCurrencyCode)
            .WithMessage("Invalid currency code. Please provide a valid ISO 4217 currency code.");

        When(x => x.AccountDto.Type == AccountType.Deposit, () =>
        {
            RuleFor(x => x.AccountDto.InterestRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Для вклада должна быть указана процентная ставка");
        });
        
        When(x => x.AccountDto.Type == AccountType.Credit, () =>
        {
            RuleFor(x => x.AccountDto.Balance)
                .LessThanOrEqualTo(0)
                .WithMessage("Credit account must have negative or zero balance");
        });
    }
}