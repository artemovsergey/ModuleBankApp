using FluentValidation;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

public class CheckAccountValidator : AbstractValidator<CheckAccountRequest>
{
    public CheckAccountValidator()
    {
        RuleFor(request => request.AccountId)
            .NotNull()
            .WithMessage("AccountId cannot be null");
    }
}