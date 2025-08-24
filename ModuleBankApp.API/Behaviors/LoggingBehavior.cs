using MediatR.Pipeline;

namespace ModuleBankApp.API.Behaviors;

public class LoggingBehavior<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest> where TRequest: notnull
{
    public Task Process(TRequest request, CancellationToken cancellationToken) 
    {
        logger.LogInformation("Start execution features: { fName}", typeof(TRequest).Name);
        return Task.CompletedTask;
    }
}

