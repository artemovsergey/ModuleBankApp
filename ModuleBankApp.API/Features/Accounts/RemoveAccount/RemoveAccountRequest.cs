using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

public record RemoveAccountRequest(Guid AccountId, Guid ClaimsId) : IRequest<MbResult<Account>>;