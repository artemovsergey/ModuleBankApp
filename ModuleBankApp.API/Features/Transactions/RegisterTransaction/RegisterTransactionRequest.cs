using MediatR;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record RegisterTransactionRequest(TransactionDto TransactionDto, Guid ClaimsId)
    : IRequest<MbResult<TransactionDto>>;