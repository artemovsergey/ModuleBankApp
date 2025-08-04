using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

    public record CreateAccountRequest(AccountDto AccountDto, Guid ClaimsId) : IRequest<MbResult<Account>>;