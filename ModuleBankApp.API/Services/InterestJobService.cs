using MediatR;
using ModuleBankApp.API.Handlers;

namespace ModuleBankApp.API.Services;

public class InterestJobService(IMediator mediator)
{
    public async Task AccrueInterest()
    {
        await mediator.Send(new AccrueInterestRequest());
    }
}

// +