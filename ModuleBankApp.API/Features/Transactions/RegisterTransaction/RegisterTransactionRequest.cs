using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

public record RegisterTransactionRequest(TransactionDto TransactionDto, Guid ClaimsId)
    : IRequest<MbResult<TransactionDto>>;