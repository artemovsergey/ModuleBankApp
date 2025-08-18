using FluentValidation;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

// ReSharper disable once UnusedType.Global
public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator(ICurrencyService currencyService)
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CreateAccountDto.Type)
            .Must(t => t is AccountType.Credit or AccountType.Deposit or AccountType.Checking)
            .WithMessage("Тип счёта должен быть 'Credit', 'Deposit' или 'Checking'");

        RuleFor(x => x.CreateAccountDto.Currency)
            .NotEmpty().WithMessage("Требуется код валюты.")
            .Length(3).WithMessage("Код валюты должен состоять из 3 символов.")
            .Must(currencyService.IsValidCurrencyCode)
            .WithMessage("Неверный код валюты (ISO 4217).");

        // Депозит/расчётный — баланс >= 0
        When(x => x.CreateAccountDto.Type is AccountType.Deposit or AccountType.Checking, () =>
        {
            RuleFor(x => x.CreateAccountDto.Balance)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .WithMessage("Баланс не может быть отрицательным для выбранного типа счёта.");
        });

        // Кредит — баланс <= 0 (если нужен кредитный лимит, вынесите его отдельно)
        When(x => x.CreateAccountDto.Type is AccountType.Credit, () =>
        {
            RuleFor(x => x.CreateAccountDto.Balance)
                .NotNull()
                .LessThanOrEqualTo(0)
                .WithMessage("Для кредитного счёта баланс должен быть неположительным (≤ 0).");
        });

        // Ставка обязательна для Deposit/Credit
        When(x => x.CreateAccountDto.Type is AccountType.Deposit or AccountType.Credit, () =>
        {
            RuleFor(x => x.CreateAccountDto.InterestRate)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Для выбранного типа счета требуется положительная процентная ставка.");
        });

        RuleFor(x => x.CreateAccountDto.Type)
            .Must(type => Enum.IsDefined(type) && type != AccountType.None)
            .WithMessage("Недопустимый тип счёта.");
    }
}