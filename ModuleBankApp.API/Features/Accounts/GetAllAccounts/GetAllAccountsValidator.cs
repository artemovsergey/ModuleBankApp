using FluentValidation;

namespace ModuleBankApp.API.Features.Accounts.GetAllAccounts;

// ReSharper disable once UnusedType.Global
public class GetAllAccountsValidator : AbstractValidator<GetAllAccountsRequest>
{
    public GetAllAccountsValidator()
    {
        RuleFor(request => request.ClaimsId)
            .NotNull()
            .WithMessage("ClaimsId cannot be null");
    }
}

// +