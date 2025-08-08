using MediatR.Pipeline;

namespace ModuleBankApp.API.Behaviors;

public class LoggingBehavior<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest> where TRequest: notnull
{
    private readonly ILogger<TRequest> _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken) 
    {
        _logger.LogInformation("Start execution features: { fName}", typeof(TRequest).Name);
        return Task.CompletedTask;
    }
}