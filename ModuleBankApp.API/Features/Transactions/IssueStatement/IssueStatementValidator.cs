using FluentValidation;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

// ReSharper disable once UnusedType.Global
public class IssueStatementValidator : AbstractValidator<IssueStatementRequest>
{
    public IssueStatementValidator()
    {
        RuleFor(request => request.AccountId)
            .NotNull()
            .WithMessage("accountId cannot be null");
    }
}

