using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.GetAllAccounts;

public record GetAllAccountsRequest(Guid ClaimsId) : IRequest<MbResult<List<Account>>>;

