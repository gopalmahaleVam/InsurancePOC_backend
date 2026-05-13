using FluentValidation;
using MediatR;

namespace Insurance.Application.Common;

/// <summary>
/// MediatR pipeline behavior for request validation.
/// Automatically validates all commands and queries before execution.
/// Throws ValidationException if validation fails, preventing invalid requests from reaching handlers.
/// </summary>
/// <typeparam name="TRequest">The request type to validate</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles request validation before passing to the actual handler.
    /// </summary>
    /// <param name="request">The incoming request</param>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The handler response if validation passes</returns>
    /// <exception cref="ValidationException">Thrown if validation fails</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Skip if no validators registered for this request type
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        // Run all validators asynchronously
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        // Throw if any validation failures
        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}