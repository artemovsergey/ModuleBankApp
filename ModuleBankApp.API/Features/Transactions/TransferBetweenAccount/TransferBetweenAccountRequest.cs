using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public record TransferBetweenAccountRequest(TransactionDto TransactionDto, Guid ClaimsId)
    : IRequest<MbResult<TransactionDto>>;