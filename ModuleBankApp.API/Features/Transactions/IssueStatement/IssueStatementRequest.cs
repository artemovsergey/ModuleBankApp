using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record IssueStatementRequest(Guid AccountId, Guid ClaimsId)
    : IRequest<MbResult<List<Transaction>>>;
    // +