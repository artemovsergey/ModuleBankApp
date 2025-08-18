using MediatR;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

public class IssueStatementHandler(
    ITransactionRepository repoTransaction,
    ILogger<IssueStatementHandler> logger) : IRequestHandler<IssueStatementRequest, MbResult<List<Transaction>>>
{
    public async Task<MbResult<List<Transaction>>> Handle(IssueStatementRequest request, CancellationToken ct)
    {
        var result = await repoTransaction.GetTransactionsByAccount(request.AccountId);
        logger.LogWarning("Transactions count by accountId {count}", result.Count);
        return MbResult<List<Transaction>>.Success(result);
    }
}

// +