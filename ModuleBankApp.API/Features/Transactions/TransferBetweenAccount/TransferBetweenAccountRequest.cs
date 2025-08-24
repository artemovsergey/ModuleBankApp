using MediatR;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record TransferBetweenAccountRequest(TransactionDto TransactionDto, Guid ClaimsId)
    : IRequest<MbResult<TransactionDto>>;
    