using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record EditAccountRequest(EditAccountDto EditAccountDto, Guid AccountId, Guid ClaimsId) : IRequest<MbResult<Account>>;

// +