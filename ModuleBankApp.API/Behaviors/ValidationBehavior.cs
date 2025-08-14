using FluentValidation;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // ReSharper disable once InvertIf
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Используем ValidateAsync вместо Validate
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(e => e != null)
                .ToList();

            // ReSharper disable once InvertIf
            if (failures.Count > 0)
            {
                var errorMessage = string.Join("; ", failures.Select(e => e.ErrorMessage));

                var responseType = typeof(TResponse);
                if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(MbResult<>))
                    throw new InvalidOperationException("TResponse must be of type MbResult<T>");

                var innerType = responseType.GetGenericArguments()[0];
                var failureMethod = typeof(MbResult<>)
                    .MakeGenericType(innerType)
                    .GetMethod(nameof(MbResult<object>.Failure))!;

                var failureResult = failureMethod.Invoke(null, [errorMessage]);
                return (TResponse)failureResult!;
            }
        }

        return await next();
    }
}


// +
