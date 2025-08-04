using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

public class RemoveAccountValidator : AbstractValidator<RemoveAccountRequest>
{
    public RemoveAccountValidator(ICurrencyService currencyService)
    {
        RuleFor(request => request.AccountId)
            .NotNull()
            .WithMessage("AccountId must be not null");
    }
}