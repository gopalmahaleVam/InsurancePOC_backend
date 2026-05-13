using FluentValidation;
using MediatR;

namespace Insurance.Application.Common
{
    
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

                var failures = _validators
                    .Select(v => v.Validate(context))
                    .SelectMany(result => result.Errors)
                    .Where(f => f != null)
                    .ToList();

            // if (failures.Any())
            // {
            //     var errors = failures
            //         .Select(e => e.ErrorMessage)
            //         .Distinct();

            //     throw new ValidationException((IEnumerable<FluentValidation.Results.ValidationFailure>)errors);
            // }
            if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
        }

        return await next();
    }
}

}