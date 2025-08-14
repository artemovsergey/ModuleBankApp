using FluentValidation;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

// ReSharper disable once UnusedType.Global
public class CheckAccountValidator : AbstractValidator<CheckAccountRequest>
{
    public CheckAccountValidator()
    {
        RuleFor(request => request.AccountId)
            .NotNull()
            .WithMessage("AccountId cannot be null")
            .NotEmpty()
            .WithMessage("AccountId cannot be empty");
    }
}

// +