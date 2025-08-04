using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

public record EditAccountRequest(EditAccountDto EditAccountDto, Guid AccountId, Guid ClaimsId) : IRequest<MbResult<Account>>;