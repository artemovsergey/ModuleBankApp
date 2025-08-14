using FluentValidation;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

// ReSharper disable once UnusedType.Global
public class RemoveAccountValidator : AbstractValidator<RemoveAccountRequest>
{
    public RemoveAccountValidator()
    {
        RuleFor(request => request.AccountId)
            .NotNull()
            .WithMessage("AccountId must be not null");
    }
}

// +