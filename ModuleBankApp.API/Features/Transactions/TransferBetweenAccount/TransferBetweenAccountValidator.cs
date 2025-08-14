using FluentValidation;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

// ReSharper disable once UnusedType.Global
public class TransferBetweenAccountValidator : AbstractValidator<TransferBetweenAccountRequest>
{
    public TransferBetweenAccountValidator(
        ICurrencyService currencyService,
        IAccountRepository accountRepo)
    {
        RuleFor(request => request.TransactionDto.Amount)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Сумма перевода должна быть больше нуля");

        RuleFor(x => x.TransactionDto.Type)
            .Must(x => x is TransactionType.Credit or TransactionType.Debit)
            .WithMessage("Тип транзакции должен быть 'Debit' или 'Credit'");

        RuleFor(request => request.TransactionDto.Currency)
            .NotEmpty()
            .Length(3)
            .Must(currencyService.IsValidCurrencyCode)
            .WithMessage("Неверный код валюты");

        RuleFor(x => x.TransactionDto.CounterPartyAccountId)
            .NotNull()
            .WithMessage("Не указан счёт получателя");

        RuleFor(x => x)
            .CustomAsync(async (req, ctx, _) =>
            {
                var sender = await accountRepo.GetAccountById(req.TransactionDto.AccountId);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (sender == null)
                {
                    ctx.AddFailure("Отправитель не найден");
                    return;
                }

                if (sender.OwnerId != req.ClaimsId)
                    ctx.AddFailure("Нет доступа к счёту");

                var receiver = await accountRepo.GetAccountById(req.TransactionDto.CounterPartyAccountId!.Value);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (receiver == null)
                {
                    ctx.AddFailure("Получатель не найден");
                    return;
                }

                if (sender.Currency != receiver.Currency)
                    ctx.AddFailure("Валюты счетов не совпадают");

                if (sender.Balance < req.TransactionDto.Amount)
                    ctx.AddFailure("Недостаточно средств");
            });
    }
}
// +