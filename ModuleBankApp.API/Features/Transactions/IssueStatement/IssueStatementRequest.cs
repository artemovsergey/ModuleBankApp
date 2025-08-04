using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

public record IssueStatementRequest(Guid accountId, Guid ClaimsId)
    : IRequest<MbResult<List<Transaction>>>;