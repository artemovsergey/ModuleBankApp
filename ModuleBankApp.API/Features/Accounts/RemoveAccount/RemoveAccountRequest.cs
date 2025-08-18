using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

public record RemoveAccountRequest(Guid AccountId) : IRequest<MbResult<Account>>;

// +