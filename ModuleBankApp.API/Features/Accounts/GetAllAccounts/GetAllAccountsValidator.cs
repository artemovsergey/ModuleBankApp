using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Accounts.GetAllAccounts;

public class GetAllAccountsValidator : AbstractValidator<GetAllAccountsRequest>
{
    public GetAllAccountsValidator(ICurrencyService currencyService)
    {
        RuleFor(request => request.ClaimsId)
            .NotNull()
            .WithMessage("ClaimsId cannot be null");
    }
}