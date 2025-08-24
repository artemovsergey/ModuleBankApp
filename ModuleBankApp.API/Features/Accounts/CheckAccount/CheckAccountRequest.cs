using MediatR;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

public record CheckAccountRequest(Guid AccountId, Guid OwnerId) : IRequest<MbResult<AccountDto>>;

