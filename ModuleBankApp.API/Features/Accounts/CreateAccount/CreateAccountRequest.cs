using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;
public record CreateAccountRequest(CreateAccountDto CreateAccountDto, Guid? ClaimsId) : IRequest<MbResult<Account>>;