using System.Diagnostics;
using MediatR;
using ModuleBankApp.API.Metrics;

namespace ModuleBankApp.API.Behaviors;

public sealed class HandlePerformancemetricBehavior<TRequest, TResponse>(HandlePerformancemetric metric) :
    IPipelineBehavior<TRequest, TResponse> where TRequest :
    IRequest<TResponse>
{
    private readonly HandlePerformancemetric _metric = metric;
    private readonly Stopwatch _timer = new();
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();
        var response =  await next();
        _timer.Stop();

        _metric.MilliSecondElapsed(_timer.ElapsedMilliseconds);
        
        return response;
    }
}