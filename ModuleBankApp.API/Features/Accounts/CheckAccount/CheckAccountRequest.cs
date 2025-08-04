using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

    public record CheckAccountRequest(Guid AccountId, Guid ClaimsId) : IRequest<MbResult<Account>>;