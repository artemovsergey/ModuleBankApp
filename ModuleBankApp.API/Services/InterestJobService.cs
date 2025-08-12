using MediatR;
using ModuleBankApp.API.Handlers;

namespace ModuleBankApp.API.Services;

public class InterestJobService
{
    private readonly IMediator _mediator;

    public InterestJobService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task AccrueInterest()
    {
        await _mediator.Send(new AccrueInterestRequest());
    }
}