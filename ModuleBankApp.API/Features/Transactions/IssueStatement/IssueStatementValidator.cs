using FluentValidation;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

public class IssueStatementValidator : AbstractValidator<IssueStatementRequest>
{
    public IssueStatementValidator()
    {
        RuleFor(request => request.accountId)
            .NotNull()
            .WithMessage("accountId cannot be null");
    }
}