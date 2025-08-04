using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public record TransferBetweenAccountRequest(TransactionTransferDto TransactionDto, Guid ClaimsId)
    : IRequest<MbResult<Transaction>>;